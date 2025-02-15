using System.Drawing;
using System.Numerics;
using CrossCutting.Core.Contract.EventBrokerage;
using ImGui.Forms.Resources;
using ImGui.Forms.Support;
using ImGuiNET;
using Logic.Domain.GraphVizManagement.Contract.DataClasses;
using UI.ScnNavigator.Abstract.DataClasses;
using UI.ScnNavigator.Components.Contract.DataClasses;
using Rectangle = Veldrid.Rectangle;
using Size = ImGui.Forms.Models.Size;

namespace UI.ScnNavigator.Components.Graphs
{
    public class GraphView : ImGui.Forms.Controls.Base.Component
    {
        private readonly Graph _graph;
        private readonly IEventBroker _eventBroker;

        private Matrix3x2 _transform;

        private bool _mouseDown;
        private Vector2 _mouseDownPosition;
        private Vector2 _mouseClickPosition;

        private bool _isSelecting;
        private GraphNode? _selectedNode;

        public float Zoom { get; private set; } = 1;

        public Vector2 SelectionPadding { get; set; } = new(2, 2);

        public GraphView(Graph graph, IEventBroker eventBroker)
        {
            _graph = graph;
            _eventBroker = eventBroker;
        }

        public override Size GetSize()
        {
            return Size.Parent;
        }

        protected override void UpdateInternal(Rectangle contentRect)
        {
            Vector2 anchorPosition = contentRect.Position + new Vector2(contentRect.Size.X / 2, contentRect.Size.Y / 2);

            if (_transform == default)
            {
                Vector2 nodePosition = new Vector2(_graph.Nodes[0].Position.X, _graph.Nodes[0].Position.Y);
                _transform = InitializeMatrix(contentRect, anchorPosition, nodePosition);
            }

            ImGuiNET.ImGui.Dummy(contentRect.Size);

            ProcessMouse(contentRect, anchorPosition);

            Vector2 transformScale = new Vector2(_transform.M11, _transform.M22);
            Vector2 absoluteContentPosition = anchorPosition + _transform.Translation;

            var context = new GraphContext(absoluteContentPosition, transformScale);

            foreach (GraphEdge edge in _graph.Edges)
                DrawEdge(edge, context);

            foreach (GraphNode node in _graph.Nodes)
                DrawNode(node, context);

            // Control legend
            ImGuiNET.ImGui.GetWindowDrawList().AddText(contentRect.Position, ImGuiNET.ImGui.GetColorU32(ImGuiCol.Text), "Move: Right Mouse Hold + Drag");
            ImGuiNET.ImGui.GetWindowDrawList().AddText(contentRect.Position + new Vector2(0, 15), ImGuiNET.ImGui.GetColorU32(ImGuiCol.Text), "Select: Left Mouse Click Block");

            // Zoom value
            Vector2 zoomPos = contentRect.Position with { Y = contentRect.Position.Y + contentRect.Size.Y - 15 };
            ImGuiNET.ImGui.GetWindowDrawList().AddText(zoomPos, ImGuiNET.ImGui.GetColorU32(ImGuiCol.Text), $"Zoom: {Zoom}");
        }

        #region Render edge

        protected virtual void DrawEdge(GraphEdge edge, GraphContext context)
        {
            var linePoints = new Vector2[edge.Points.Length];
            for (var i = 0; i < linePoints.Length; i++)
            {
                var edgePointPos = new Vector2(edge.Points[i].X, edge.Points[i].Y);
                linePoints[i] = TransformPointToGraph(edgePointPos, context);
            }

            uint edgeColor = GetEdgeColor(edge);
            int edgeThickness = GetEdgeThickness(edge);

            ImGuiNET.ImGui.GetWindowDrawList().AddPolyline(ref linePoints[0], linePoints.Length, edgeColor, ImDrawFlags.None, edgeThickness);
            ImGuiNET.ImGui.GetWindowDrawList().AddCircleFilled(linePoints[0], edgeThickness + 2, (edgeColor & 0x00FFFFFF) | 0xFF000000);
        }

        protected virtual uint GetEdgeColor(GraphEdge edge) => ImGuiNET.ImGui.GetColorU32(ImGuiCol.Border);

        protected virtual int GetEdgeThickness(GraphEdge edge) => 1;

        #endregion

        #region Render node

        private void DrawNode(GraphNode node, GraphContext context)
        {
            var nodePosition = new Vector2(node.Position.X, node.Position.Y);
            var nodeSize = new Vector2(node.Width, node.Height);

            Vector2 absoluteNodePosition = TransformPointToGraph(nodePosition, context);
            Vector2 absoluteNodeSize = nodeSize * context.TransformScale;

            uint nodeColor = GetNodeColor(node);
            ImGuiNET.ImGui.GetWindowDrawList().AddRectFilled(absoluteNodePosition, absoluteNodePosition + absoluteNodeSize, nodeColor);

            if (Enabled)
                UpdateSelectedNode(node, absoluteNodePosition, absoluteNodeSize);

            if (_selectedNode == node)
            {
                Vector2 selectionPos = absoluteNodePosition - SelectionPadding;
                Vector2 selectionSize = absoluteNodeSize + SelectionPadding * 2;

                ImGuiNET.ImGui.GetWindowDrawList().AddRectFilled(selectionPos, selectionPos + selectionSize, 0xC0000000 | (nodeColor & 0x00FFFFFF));
            }

            if (Zoom < 0.5)
                return;

            Vector2 labelSize = TextMeasurer.MeasureText(node.Label);
            Vector2 labelPos = absoluteNodePosition + (absoluteNodeSize - labelSize) / 2;

            ImGuiNET.ImGui.GetWindowDrawList().AddText(labelPos, ImGuiNET.ImGui.GetColorU32(ImGuiCol.Text), node.Label);
        }

        protected virtual uint GetNodeColor(GraphNode node) => ImGuiNET.ImGui.GetColorU32(ImGuiCol.Border);

        private void UpdateSelectedNode(GraphNode node, Vector2 absolutePosition, Vector2 absoluteSize)
        {
            if (IsHoveredCore() && _mouseClickPosition != Vector2.Zero && _selectedNode != node)
            {
                var nodeRectangle = new RectangleF(absolutePosition.X, absolutePosition.Y, absoluteSize.X, absoluteSize.Y);
                if (nodeRectangle.Contains(_mouseClickPosition.X, _mouseClickPosition.Y) && !_isSelecting)
                    ChangeSelectedNode(node);
            }
        }

        #endregion

        protected Vector2 TransformPointToGraph(Vector2 point, GraphContext context)
        {
            return context.AbsoluteContentPosition + point * context.TransformScale;
        }

        private void ProcessMouse(Rectangle contentRect, Vector2 anchorPosition)
        {
            ImGuiIOPtr io = ImGuiNET.ImGui.GetIO();

            if (io.MouseWheel != 0 && contentRect.Contains((int)io.MousePos.X, (int)io.MousePos.Y))
            {
                Vector2 translatedComponentPosition = anchorPosition + _transform.Translation;

                float zoomValue = io.MouseWheel / 8;
                if (Zoom + zoomValue < 0)
                    zoomValue = -Zoom;

                if (Zoom > 0 || zoomValue >= 0)
                {
                    Zoom += zoomValue;

                    Vector2 scale = Vector2.One + new Vector2(Math.Abs(zoomValue));
                    Vector2 translatedMousePosition = io.MousePos + _transform.Translation;

                    var scaleMatrix = Matrix3x2.CreateScale(scale, translatedMousePosition - translatedComponentPosition);
                    if (zoomValue < 0)
                        Matrix3x2.Invert(scaleMatrix, out scaleMatrix);

                    _transform *= scaleMatrix;
                }
            }

            _mouseClickPosition = Vector2.Zero;
            if (ImGuiNET.ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                _mouseClickPosition = ImGuiNET.ImGui.GetMousePos();

            if (!_mouseDown && IsHoveredCore() && ImGuiNET.ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                _mouseDownPosition = ImGuiNET.ImGui.GetMousePos();
                _mouseDown = true;

                ImGuiNET.ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeNESW);
            }

            if (ImGuiNET.ImGui.IsMouseReleased(ImGuiMouseButton.Right))
            {
                _mouseDownPosition = Vector2.Zero;
                _mouseDown = false;

                ImGuiNET.ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
            }

            if (_mouseDown)
            {
                _transform *= Matrix3x2.CreateTranslation(ImGuiNET.ImGui.GetMousePos() - _mouseDownPosition);
                _mouseDownPosition = ImGuiNET.ImGui.GetMousePos();
            }
        }

        private Matrix3x2 InitializeMatrix(Rectangle contentRect, Vector2 anchorPosition, Vector2 positionToAnchor)
        {
            Matrix3x2 transform = new(1, 0, 0, 1, 0, 0);

            transform *= Matrix3x2.CreateTranslation(-positionToAnchor);

            Vector2 leftMiddlePosition = contentRect.Position + new Vector2(0, contentRect.Height / 2f);
            Vector2 anchorBasePosition = anchorPosition - leftMiddlePosition;
            transform *= Matrix3x2.CreateTranslation(-anchorBasePosition);

            Vector2 translatedAnchorPosition = anchorPosition + _transform.Translation;
            Vector2 translatedZoomPosition = leftMiddlePosition + _transform.Translation;

            transform *= Matrix3x2.CreateScale(Vector2.One - new Vector2(.25f), translatedZoomPosition - translatedAnchorPosition);

            return transform;
        }

        private void ChangeSelectedNode(GraphNode node)
        {
            _isSelecting = true;

            OnSelectedGraphNodeChanged(node);
            SetSelectedNode(node);

            _isSelecting = false;
        }

        protected void SetSelectedNode(GraphNode node)
        {
            _selectedNode = node;
        }

        protected virtual void OnSelectedGraphNodeChanged(GraphNode node)
        {
            if (_selectedNode != node)
                _eventBroker.Raise(new SelectedGraphNodeChangedMessage(node));
        }
    }
}

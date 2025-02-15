using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.Messages;
using ImGui.Forms;
using ImGui.Forms.Localization;
using UI.ScnNavigator;
using UI.ScnNavigator.Forms.Contract;
using UI.ScnNavigator.Resources.Contract;
using UI.ScnNavigator.Resources.Contract.Enums;

KernelLoader loader = new();
ICoCoKernel kernel = loader.Initialize();

var eventBroker = kernel.Get<IEventBroker>();
eventBroker.Raise(new InitializeApplicationMessage());

var localizer = kernel.Get<ILocalizer>();

var app = new Application(localizer);

var fontProvider = kernel.Get<IFontProvider>();

fontProvider.RegisterFont(Font.Roboto);
fontProvider.RegisterFont(Font.NotoJp);

var formFactory = kernel.Get<IFormFactory>();

Form form = formFactory.CreateMainForm();
form.DefaultFont = fontProvider.GetFont(Font.Roboto);

app.Execute(form);

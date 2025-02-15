namespace Logic.Business.RenderingManagement.Contract.Metrics.Enums
{
    public enum MetricDetailType
    {
        InvalidOpenTag,
        InvalidCloseTag,

        InvalidCharacter,

        InvalidComma,
        InvalidExclamationPoint,
        InvalidDot,
        InvalidEllipses,
        InvalidQuestionMark,

        ContinuousSpaces,
        MisplacedSpace,

        EmptyLine,
        AutoWrappedLine,
        TooManyLines
    }
}

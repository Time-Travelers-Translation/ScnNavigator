using Logic.Business.RenderingManagement.Contract.Layouts.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics;
using Logic.Business.RenderingManagement.Contract.Metrics.DataClasses;
using Logic.Business.RenderingManagement.Contract.Metrics.Enums;
using Logic.Business.RenderingManagement.Contract.Parsers.DataClasses;

namespace Logic.Business.RenderingManagement.Metrics
{
    internal class PunctuationMetricStrategy : BaseMetricStrategy, IPunctuationMetricStrategy
    {
        public override IList<MetricDetailData> Validate(TextLayoutData layout, IList<CharacterData> characters)
        {
            var result = new List<MetricDetailData>();

            foreach (TextLayoutLineData layoutLine in layout.Lines)
            {
                var isLineStart = true;
                for (var i = 0; i < layoutLine.Characters.Count; i++)
                {
                    if (!layoutLine.Characters[i].Character.IsVisible)
                        continue;

                    if (TryGetCharacter(layoutLine.Characters[i].Character, out ushort character))
                    {
                        switch (character)
                        {
                            case '.':
                                ValidateDot(layoutLine, ref i, isLineStart, result);
                                break;

                            case '?':
                                ValidateQuestionMark(layoutLine, ref i, isLineStart, result);
                                break;

                            case '!':
                                ValidateExclamationPoint(layoutLine, ref i, isLineStart, result);
                                break;

                            case ',':
                                ValidateComma(layoutLine, ref i, isLineStart, result);
                                break;
                        }
                    }

                    isLineStart = false;
                }
            }

            return result;
        }

        private void ValidateDot(TextLayoutLineData line, ref int index, bool isLineStart, IList<MetricDetailData> details)
        {
            bool isEllipses = TryGetCharacter(line, index + 1, out ushort nextCharacter) && nextCharacter is '.'
                && TryGetCharacter(line, index + 2, out nextCharacter) && nextCharacter is '.';

            if (isEllipses)
            {
                ValidateEllipses(line, ref index, isLineStart, details);
                return;
            }

            // Dot at start of line
            if (isLineStart)
            {
                // Dot invalid if followed by any amount of punctuations
                int origIndex = index;

                while (TryGetCharacter(line, index + 1, out nextCharacter))
                {
                    if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                        break;

                    index++;
                }

                details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidDot));
                return;
            }

            if (TryGetCharacter(line, index - 1, out ushort previousCharacter))
            {
                if (previousCharacter is ' ' or '"')
                {
                    details.Add(CreateMetricDetail(line.Characters[index - 1], line.Characters[index], MetricDetailType.InvalidDot));
                    return;
                }
            }

            if (TryGetCharacterData(line, index + 1, out CharacterData? nextCharacterData))
            {
                if (!nextCharacterData.IsVisible)
                    return;

                if (TryGetCharacter(nextCharacterData, out nextCharacter))
                {
                    if (nextCharacter is ' ' or '"')
                        return;

                    if (nextCharacter is '.' or '!' or '?' or ',')
                    {
                        // Dot invalid if followed by any amount of punctuations
                        int origIndex = index;

                        index++;
                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 0)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidDot));

                        return;
                    }
                }

                // Dot is invalid if it's not followed by a space, or line break
                details.Add(CreateMetricDetail(line.Characters[index], line.Characters[index + 1], MetricDetailType.InvalidDot));
            }
        }

        private void ValidateEllipses(TextLayoutLineData line, ref int index, bool isLineStart, IList<MetricDetailData> details)
        {
            index += 2;
            if (TryGetCharacter(line, index - 3, out ushort previousCharacter))
            {
                if (previousCharacter is ' ' or '"')
                {
                    details.Add(CreateMetricDetail(line.Characters[index - 3], line.Characters[index], MetricDetailType.InvalidEllipses));
                    return;
                }
            }

            if (TryGetCharacterData(line, index + 1, out CharacterData? nextCharacterData))
            {
                if (!nextCharacterData.IsVisible)
                    return;

                if (TryGetCharacter(nextCharacterData, out ushort nextCharacter))
                {
                    if(nextCharacter is '"')
                        return;

                    if (nextCharacter is ' ')
                    {
                        if (isLineStart)
                        {
                            details.Add(CreateMetricDetail(line.Characters[index - 2], line.Characters[index + 1], MetricDetailType.InvalidEllipses));
                            return;
                        }

                        if (TryGetCharacterData(line, index + 2, out nextCharacterData))
                        {
                            index++;

                            if (!nextCharacterData.IsVisible)
                            {
                                details.Add(CreateMetricDetail(line.Characters[index - 3], line.Characters[index + 1], MetricDetailType.InvalidEllipses));
                                return;
                            }

                            if (TryGetCharacter(nextCharacterData, out nextCharacter))
                            {
                                if(char.IsUpper((char)nextCharacter))
                                    return;

                                details.Add(CreateMetricDetail(line.Characters[index - 3], line.Characters[index + 1], MetricDetailType.InvalidEllipses, MetricDetailLevel.Warn));
                                return;
                            }

                            return;
                        }
                    }

                    if (nextCharacter is '?' or '!')
                    {
                        // Ellipses valid if followed by a single exclamation point or question mark
                        int origIndex = index++;
                        var threshold = 1;

                        if (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is '!')
                            {
                                index++;
                                threshold++;
                            }
                            else if (nextCharacter is not ' ')
                            {
                                details.Add(CreateMetricDetail(line.Characters[origIndex - 2], line.Characters[index + 1], MetricDetailType.InvalidExclamationPoint));
                                return;
                            }
                        }

                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > threshold)
                            details.Add(CreateMetricDetail(line.Characters[origIndex - 2], line.Characters[index], MetricDetailType.InvalidEllipses));

                        return;
                    }

                    if (nextCharacter is '.' or ',')
                    {
                        // Ellipses invalid if followed by any amount punctuation
                        int origIndex = index;

                        index++;
                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 0)
                            details.Add(CreateMetricDetail(line.Characters[origIndex - 2], line.Characters[index], MetricDetailType.InvalidEllipses));

                        return;
                    }
                }

                // Ellipses is invalid if it's not followed by a space, or line break
                if (!isLineStart)
                    details.Add(CreateMetricDetail(line.Characters[index - 2], line.Characters[index + 1], MetricDetailType.InvalidEllipses));
            }
        }

        private void ValidateQuestionMark(TextLayoutLineData line, ref int index, bool isLineStart, IList<MetricDetailData> details)
        {
            // Question mark at start of line
            if (isLineStart)
            {
                // Question mark invalid if followed by any amount of punctuations
                int origIndex = index;

                while (TryGetCharacter(line, index + 1, out ushort nextCharacter))
                {
                    if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                        break;

                    index++;
                }

                details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidQuestionMark));
                return;
            }

            if (TryGetCharacter(line, index - 1, out ushort previousCharacter))
            {
                if (previousCharacter is ' ' or '"')
                {
                    details.Add(CreateMetricDetail(line.Characters[index - 1], line.Characters[index], MetricDetailType.InvalidQuestionMark));
                    return;
                }
            }

            if (TryGetCharacterData(line, index + 1, out CharacterData? nextCharacterData))
            {
                if (!nextCharacterData.IsVisible)
                    return;

                if (TryGetCharacter(nextCharacterData, out ushort nextCharacter))
                {
                    if (nextCharacter is ' ' or '"')
                        return;

                    if (nextCharacter is '!')
                    {
                        // Question mark valid if followed by a single exclamation point
                        int origIndex = index++;

                        if (TryGetCharacter(line, index + 1, out nextCharacter) && nextCharacter is not ' ' and not '"')
                        {
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index + 1], MetricDetailType.InvalidQuestionMark));
                            return;
                        }

                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 1)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidQuestionMark));

                        return;
                    }

                    if (nextCharacter is '.' or '?' or ',')
                    {
                        // Question mark invalid if followed by any amount of punctuations
                        int origIndex = index;

                        index++;
                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 0)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidQuestionMark));

                        return;
                    }
                }

                // Question mark is invalid if it's not followed by a space, or line break
                details.Add(CreateMetricDetail(line.Characters[index], line.Characters[index + 1], MetricDetailType.InvalidQuestionMark));
            }
        }

        private void ValidateExclamationPoint(TextLayoutLineData line, ref int index, bool isLineStart, IList<MetricDetailData> details)
        {
            // Exclamation point at start of line
            if (isLineStart)
            {
                // Exclamation point invalid if followed by any amount of punctuations
                int origIndex = index;

                while (TryGetCharacter(line, index + 1, out ushort nextCharacter))
                {
                    if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                        break;

                    index++;
                }

                details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidExclamationPoint));
                return;
            }

            if (TryGetCharacter(line, index - 1, out ushort previousCharacter))
            {
                if (previousCharacter is ' ' or '"')
                {
                    details.Add(CreateMetricDetail(line.Characters[index - 1], line.Characters[index], MetricDetailType.InvalidExclamationPoint));
                    return;
                }
            }

            if (TryGetCharacterData(line, index + 1, out CharacterData? nextCharacterData))
            {
                if (!nextCharacterData.IsVisible)
                    return;

                if (TryGetCharacter(nextCharacterData, out ushort nextCharacter))
                {
                    if (nextCharacter is ' ' or '"')
                        return;

                    if (nextCharacter is '!')
                    {
                        // Exclamation point valid if followed by a single exclamation point
                        int origIndex = index++;

                        if (TryGetCharacter(line, index + 1, out nextCharacter) && nextCharacter is not ' ')
                        {
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index + 1], MetricDetailType.InvalidExclamationPoint));
                            return;
                        }

                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex == 1)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidExclamationPoint, MetricDetailLevel.Warn));

                        if (index - origIndex > 1)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidExclamationPoint));

                        return;
                    }

                    if (nextCharacter is '.' or '?' or ',')
                    {
                        // Exclamation point invalid if followed by any amount of punctuations
                        int origIndex = index;

                        index++;
                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 0)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidExclamationPoint));

                        return;
                    }
                }

                // Exclamation point is invalid if it's not followed by a space, or line break
                details.Add(CreateMetricDetail(line.Characters[index], line.Characters[index + 1], MetricDetailType.InvalidExclamationPoint));
            }
        }

        private void ValidateComma(TextLayoutLineData line, ref int index, bool isLineStart, IList<MetricDetailData> details)
        {
            // Comma at start of line
            if (isLineStart)
            {
                // Comma invalid if followed by any amount of punctuations
                int origIndex = index;

                while (TryGetCharacter(line, index + 1, out ushort nextCharacter))
                {
                    if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                        break;

                    index++;
                }

                details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidComma));
                return;
            }

            var isPreviousNumber = false;
            if (TryGetCharacter(line, index - 1, out ushort previousCharacter))
            {
                // Comma is invalid if it's preceded only by space, or punctuation (shouldn't happen)
                if (previousCharacter is ' ' or '"')
                {
                    details.Add(CreateMetricDetail(line.Characters[index - 1], line.Characters[index], MetricDetailType.InvalidComma));
                    return;
                }

                isPreviousNumber = previousCharacter is >= '0' and <= '9';
            }

            if (TryGetCharacterData(line, index + 1, out CharacterData? nextCharacterData))
            {
                if (!nextCharacterData.IsVisible)
                    return;

                if (TryGetCharacter(nextCharacterData, out ushort nextCharacter))
                {
                    if (nextCharacter is ' ' or '"')
                        return;

                    if (isPreviousNumber && nextCharacter is >= '0' and <= '9')
                        return;

                    if (nextCharacter is '.' or '!' or '?' or ',')
                    {
                        // Comma invalid if followed by any amount of punctuations
                        int origIndex = index;

                        index++;
                        while (TryGetCharacter(line, index + 1, out nextCharacter))
                        {
                            if (nextCharacter is not '.' and not '!' and not '?' and not ',')
                                break;

                            index++;
                        }

                        if (index - origIndex > 0)
                            details.Add(CreateMetricDetail(line.Characters[origIndex], line.Characters[index], MetricDetailType.InvalidComma));

                        return;
                    }
                }

                // Comma is invalid if it's not followed by a space, number (if preceded by a number), or line break
                details.Add(CreateMetricDetail(line.Characters[index], line.Characters[index + 1], MetricDetailType.InvalidComma));
                return;
            }

            // Comma is invalid if it's not followed by a space, number (if preceded by a number), or line break
            details.Add(CreateMetricDetail(line.Characters[index], MetricDetailType.InvalidComma));
        }
    }
}

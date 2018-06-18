using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Visualizer.Model
{
    public class TagStrip
    {
        public List<TagText> TagTexts { get; set; }

        public ObservableCollection<Tag> Tags { get; set; }

        public TagStrip()
        {
            TagTexts = new List<TagText>();
            Tags = new ObservableCollection<Tag>();
        }

        public void Set(DateTime begin, DateTime end, string text)
        {
            var tagText = TagTexts.SingleOrDefault(o => o.Text == text);
            if (tagText == null)
            {
                tagText = new TagText {Text = text};
                TagTexts.Add(tagText);
            }

            Tags.RemoveAll(o => o.Begin >= begin && o.End <= end);
            var beginTag = Tags.SingleOrDefault(o => o.Begin <= begin && o.End >= begin);
            if (beginTag != null)
            {
                if (beginTag.End > end)
                {
                    if (beginTag.TagText == tagText)
                        return;
                    Tags.Add(new Tag {Begin = end, End = beginTag.End, TagText = beginTag.TagText});
                    beginTag.End = begin;
                    beginTag = new Tag {Begin = begin, End = end, TagText = tagText};
                    Tags.Add(beginTag);
                }
                else
                {
                    if (beginTag.TagText == tagText)
                    {
                        beginTag.End = end;
                    }
                    else
                    {
                        beginTag.End = begin;
                        beginTag = new Tag {Begin = begin, End = end, TagText = tagText};
                        Tags.Add(beginTag);
                    }
                }
            }
            else
            {
                beginTag = new Tag { Begin = begin, End = end, TagText = tagText };
                Tags.Add(beginTag);
            }

            var endTag = Tags.SingleOrDefault(o => o.Begin <= end && o.End >= end && o != beginTag);
            if (endTag != null)
            {
                if (beginTag.TagText == endTag.TagText)
                {
                    beginTag.End = endTag.End;
                    Tags.Remove(endTag);
                }
                else
                {
                    endTag.Begin = end;
                }
            }
        }
    }
}

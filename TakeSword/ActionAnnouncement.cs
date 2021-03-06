﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TakeSword
{
    public class ActionAnnouncement
    {
        public bool For(TargetType relationship)
        {
            return Relationship == relationship;
        }
        public ActionAnnouncement(object content, ActionOutcome? outcome, TargetType relationship)
        {
            Content = content;
            Outcome = outcome;
            Relationship = relationship;
        }
        
        public bool IsSuccessful<T>(
            [NotNullWhen(true)]out T? typedContent,
            TargetType relationship, bool successful=true) where T : class
        {

            if (Relationship != relationship || Content == null || Outcome == null || Outcome != successful)
            {
                typedContent = null;
                return false;
            }
            if (Content != null && Content is T typedResult)
            {
                typedContent = typedResult;
                return true;
            }
            else
            {
                typedContent = null;
                return false;
            }
        }
        public object Content { get; private set; }

        // Can be null if the action is still pending
        public ActionOutcome? Outcome { get; private set; }
        public TargetType Relationship { get; private set; }
    }
}

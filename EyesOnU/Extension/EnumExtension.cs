using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Extension
{
    public class CodeAttribute : Attribute
    {
        public string Code { get; protected set; }

        public CodeAttribute(string value)
        {
            this.Code = value;
        }
    }
    public class PostfixAttribute : Attribute
    {
        public string Postfix { get; protected set; }

        public PostfixAttribute(string value)
        {
            this.Postfix = value;
        }
    }
    internal static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static string GetPostfix(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(PostfixAttribute)) as PostfixAttribute;
            return attribute == null ? value.ToString() : attribute.Postfix;
        }
    }
}

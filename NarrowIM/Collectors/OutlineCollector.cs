using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using NarrowIM.Common;

namespace NarrowIM.Collectors
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OutlineCollector : CollectorBase
    {
        /// <summary></summary>
        protected override int  CommandId  => 4129;
        /// <summary></summary>
        protected override Guid CommandSet => new Guid("70ae706d-5d3f-463e-b2b2-122e22bce1b2");
        /// <summary>
        /// Initializes a new instance of the <see cref="Outline"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private OutlineCollector(NarrowIMPackage package) : base(package)
        {
        }
        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OutlineCollector Instance
        {
            get;
            private set;
        }
        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(NarrowIMPackage package)
        {
            Instance = new OutlineCollector(package);
        }
        /// <summary>
        /// </summary>
        public override IEnumerable<Candidate> Collect()
        {
            Document doc = GetActiveDocument();
            // check doc
            if (doc == null)
            {
                return new List<Candidate>(0);
            }

            FileCodeModel model = doc.ProjectItem.FileCodeModel;
            // not source code
            if (model == null)
            {
                return new List<Candidate>(0);
            }
            // get functions and properties
            List<object> methods = GetFunctions(model.CodeElements);
            // convert to candidates
            IEnumerable<Candidate> candidates = methods.Select(v => ToCandidate(v));

            return candidates;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private Candidate ToCandidate(object obj)
        {
            dynamic value = obj;
            return new Candidate {
                Word  = value.Name,
                Abbr  = ConvertToAbbr(value),
                Value = value,
                Func  = Jump,
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Jump(object value)
        {
            int lineNo = ((dynamic)value).StartPoint.Line;
            EnvDTE.TextSelection selection = GetActiveDocument().Selection as EnvDTE.TextSelection;
            selection.MoveToDisplayColumn(lineNo, 1, false);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<object> GetFunctions(CodeElements elements, List<object> list = null)
        {
            list = list ?? new List<object>();
            foreach (CodeElement element in elements)
            {
                if (element.Kind == vsCMElement.vsCMElementFunction || element.Kind == vsCMElement.vsCMElementProperty)
                {
                    list.Add(element);
                }
                list = GetFunctions(element.Children, list);
            }

            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private string ConvertToAbbr(object obj)
        {
            // property
            if (obj is CodeProperty)
            {
                CodeProperty prop = obj as CodeProperty;
                return ConvertToMark(prop.Access) + " " + prop.Name;
            }
            // function
            if (obj is CodeFunction)
            {
                CodeFunction func = obj as CodeFunction;
                StringBuilder buf = new StringBuilder();

                foreach(dynamic param in func.Parameters)
                {
                    if (buf.Length != 0)
                    {
                        buf.Append(", ");
                    }

                    string type = param.Type.AsString;
                    type = type.Substring(type.LastIndexOf('.') + 1);
                    buf.Append(type).Append(" ");
                    buf.Append(param.Name);
                }

                buf.Insert(0, string.Format("{0} {1} (", ConvertToMark(func.Access), func.Name));
                buf.Append(")");

                return buf.ToString();
            }

            return obj.ToString();
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        private string ConvertToMark(vsCMAccess access)
        {
            switch (access)
            {
                case vsCMAccess.vsCMAccessAssemblyOrFamily:
                    return "  ";
                case vsCMAccess.vsCMAccessDefault:
                    return "  ";
                case vsCMAccess.vsCMAccessPrivate:
                    return "- ";
                case vsCMAccess.vsCMAccessProject:
                    return "  ";
                case vsCMAccess.vsCMAccessProjectOrProtected:
                    return "  ";
                case vsCMAccess.vsCMAccessProtected:
                    return "#";
                case vsCMAccess.vsCMAccessPublic:
                    return "+";
                case vsCMAccess.vsCMAccessWithEvents:
                    return "  ";
                default:
                    return "  ";
            }
        }
    }
}

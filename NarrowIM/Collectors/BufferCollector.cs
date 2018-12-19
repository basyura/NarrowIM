using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NarrowIM.Common;

namespace NarrowIM.Collectors
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BufferCollector : CollectorBase
    {
        /// <summary></summary>
        protected override int  CommandId  => 0x0100;
        /// <summary></summary>
        protected override Guid CommandSet => new Guid("70ae706d-5d3f-463e-b2b2-122e22bce1b2");
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferCollector"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private BufferCollector(NarrowIMPackage package) : base(package)
        {
        }
        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static BufferCollector Instance
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
            Instance = new BufferCollector(package);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Candidate> Collect()
        {
            // 直近の歴をもとに開いているフィルを並べる必要あり

            Document activeDoc = GetActiveDocument();
            Uri uri = new Uri(Path.GetDirectoryName(activeDoc.FullName));

            List<string> files = new List<string>(GetMruBuffers());
            files.Remove(activeDoc.FullName);
            files.Add(activeDoc.FullName);

            IEnumerable<Candidate> candidates = files.Select(v => new Candidate {
                Word        = Path.GetFileName(v),
                Description = uri.MakeRelativeUri(new Uri(Path.GetDirectoryName(v))).ToString(),
                Value       = v,
                Func        = (vl) => {
                    try
                    {
                        Env.ItemOperations.OpenFile(v);
                    }
                    catch
                    {
                        // 例外が発生する場合があるけど理由がよくわからない
                    }
                    return true;
                },
            });

            return candidates;
        }
    }
}

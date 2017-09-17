using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NarrowIM.Common;
using NarrowIM.Views;

namespace NarrowIM
{
    public abstract class CollectorBase
    {
        /// <summary></summary>
        private Package _package;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        public CollectorBase(Package package)
        {
            _package = package ?? throw new ArgumentNullException("package");

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService service)
            {
                var menuItem = new MenuCommand(CallBack, new CommandID(CommandSet, CommandId));
                service.AddCommand(menuItem);
            }
        }
        /// <summary></summary>
        protected abstract int  CommandId  { get; }
        /// <summary></summary>
        protected abstract Guid CommandSet { get; }
        /// <summary></summary>
        protected IServiceProvider ServiceProvider { get { return _package; } }
        /// <summary></summary>
        protected DTE2 Env {  get { return ServiceProvider.GetService(typeof(SDTE)) as DTE2; } }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Candidate> Collect();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Document GetActiveDocument()
        {
            return Env.ActiveDocument;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<Document> GetDocuments()
        {
            List<Document> docs = new List<Document>();
            foreach (Document doc in Env.Documents)
            {
                if (doc.ActiveWindow == null)
                {
                    continue;
                }
                docs.Add(doc);
            }

            return docs;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CallBack(object sender, EventArgs e)
        {
            ShowDialog(this);
        }
        /// <summary>
        /// </summary>
        private void ShowDialog(CollectorBase collector)
        {
            DTE2 dte = Env;
            // no doc
            if (dte.ActiveDocument == null)
            {
                return;
            }
            // get editor window
            Window editor = dte.ActiveDocument.ActiveWindow;
            // narrow window
            NarrowWindow w = new NarrowWindow(dte, collector);
            w.Left = editor.Left + (editor.Width  / 2 - w.Width  / 2);
            w.Top  = editor.Top  + (editor.Height / 2 - w.Height / 2);
            w.ShowDialog();
        }
    }
}

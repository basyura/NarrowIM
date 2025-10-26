using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NarrowIM.Collectors;

namespace NarrowIM
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0")] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(NarrowIMPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class NarrowIMPackage : Package, IVsSelectionEvents
    {
        /// <summary>
        /// Command1Package GUID string.
        /// </summary>
        public const string PackageGuidString = "53557b66-07a0-432c-b077-cc90d1433baa";
        /// <summary></summary>
        private List<string> _mruBuffers = new List<string>();
        private IVsMonitorSelection _monitorSelection;
        private uint _selectionCookie;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferCollector"/> class.
        /// </summary>
        public NarrowIMPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        /// <summary> </summary>
        public IEnumerable<string> MRUBufferes
        {
            get
            {
                var dte = GetService(typeof(SDTE)) as EnvDTE80.DTE2;
                HashSet<string> docs = new HashSet<string>();
                foreach (Document doc in dte.Documents)
                {
                    if (doc.ActiveWindow == null)
                    {
                        continue;
                    }
                    docs.Add(doc.FullName);
                }

                _mruBuffers = _mruBuffers.Where(v => docs.Contains(v)).ToList();
    
                return new List<string>(_mruBuffers);
            }
        }
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            BufferCollector.Initialize(this);
            OutlineCollector.Initialize(this);

            var dte = GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            if (dte == null)
            {
                base.Initialize();
                return;
            }

            foreach (Document doc in dte.Documents)
            {
                _mruBuffers.Add(doc.FullName);
            }

            // VS Shell の選択イベントを購読してアクティブ ドキュメント変更を検出
            _monitorSelection = GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
            if (_monitorSelection != null)
            {
                _monitorSelection.AdviseSelectionEvents(this, out _selectionCookie);
            }

            base.Initialize();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GotFocus"></param>
        /// <param name="LostFocus"></param>
        private void NarrowIMPackage_WindowActivated(Window GotFocus, Window LostFocus)
        {
            Document doc = GotFocus.Document;
            if (doc == null)
            {
                return;
            }
            _mruBuffers.Remove(doc.FullName);
            _mruBuffers.Insert(0, doc.FullName);
        }

        // IVsSelectionEvents implementation
        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
                                       IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            try
            {
                var dte = GetService(typeof(SDTE)) as EnvDTE80.DTE2;
                var active = dte?.ActiveDocument;
                if (active != null && !string.IsNullOrEmpty(active.FullName))
                {
                    _mruBuffers.Remove(active.FullName);
                    _mruBuffers.Insert(0, active.FullName);
                }
            }
            catch
            {
                // ignore and continue
            }
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _selectionCookie != 0 && _monitorSelection != null)
                {
                    _monitorSelection.UnadviseSelectionEvents(_selectionCookie);
                    _selectionCookie = 0;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}

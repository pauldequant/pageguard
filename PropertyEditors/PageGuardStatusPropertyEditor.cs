using Umbraco.Cms.Core.PropertyEditors;

namespace PageGuard.PropertyEditors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Cms.Core.IO;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    [DataEditor("Page Guard", "Page Guard: Status", "PageGuardStatus", Icon = "icon-shield", Group = "Page Guard", ValueEditorIsReusable = true)]
    public class PageGuardStatusPropertyEditor : DataEditor
    {
        private readonly IEditorConfigurationParser _editorConfigurationParser;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _textService;

        public PageGuardStatusPropertyEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            ILocalizedTextService textService,
            IIOHelper ioHelper,
            IEditorConfigurationParser editorConfigurationParser)
            : base(dataValueEditorFactory)
        {
            this._textService = textService;
            this._ioHelper = ioHelper;
            this._editorConfigurationParser = editorConfigurationParser;
            this.SupportsReadOnly = true;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => (IConfigurationEditor)new ValueListConfigurationEditor(this._textService, this._ioHelper, this._editorConfigurationParser);

        /// <inheritdoc />
        protected override IDataValueEditor CreateValueEditor() => (IDataValueEditor)this.DataValueEditorFactory.Create<MultipleValueEditor>((object)this.Attribute);
    }
}

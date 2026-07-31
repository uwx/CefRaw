using RawCef;

namespace Xilium.CefGlue.Common.Shared
{
    internal abstract class CommonCefApp : CefApp
    {
        private readonly CustomScheme[] _customSchemes;

        internal CommonCefApp(CustomScheme[] customSchemes = null)
        {
            _customSchemes = customSchemes;
        }

        public override void OnRegisterCustomSchemes(ICefSchemeRegistrar registrar)
        {
            if (_customSchemes != null)
            {
                foreach (var scheme in _customSchemes)
                {
                    registrar!.AddCustomScheme(scheme.SchemeName, (int)scheme.Options);
                }
            }
        }
    }
}

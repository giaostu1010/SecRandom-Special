namespace SecRandom.Langs.SettingsPages.LinkageSettingsPage
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {
        private static global::System.Resources.ResourceManager resourceMan;
        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public Resources()
        {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp =
                        new global::System.Resources.ResourceManager("SecRandom.Langs.SettingsPages.LinkageSettingsPage.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }

                return resourceMan;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture
        {
            get => resourceCulture;
            set => resourceCulture = value;
        }

        public static string FeatureInDevelopment => ResourceManager.GetString("FeatureInDevelopment", resourceCulture);
        public static string CsesImport => ResourceManager.GetString("CsesImport", resourceCulture);
        public static string ScheduleImport => ResourceManager.GetString("ScheduleImport", resourceCulture);
        public static string ScheduleImportDesc => ResourceManager.GetString("ScheduleImportDesc", resourceCulture);
        public static string NoScheduleImported => ResourceManager.GetString("NoScheduleImported", resourceCulture);
        public static string ImportFromCsesFile => ResourceManager.GetString("ImportFromCsesFile", resourceCulture);
        public static string ImportFromCsesFileDesc => ResourceManager.GetString("ImportFromCsesFileDesc", resourceCulture);
        public static string ImportFromCsesClipboard => ResourceManager.GetString("ImportFromCsesClipboard", resourceCulture);
        public static string ImportFromCsesClipboardDesc => ResourceManager.GetString("ImportFromCsesClipboardDesc", resourceCulture);
        public static string OpenCurrentCsesConfig => ResourceManager.GetString("OpenCurrentCsesConfig", resourceCulture);
        public static string OpenCurrentCsesConfigDesc => ResourceManager.GetString("OpenCurrentCsesConfigDesc", resourceCulture);
        public static string ClearCsesSchedule => ResourceManager.GetString("ClearCsesSchedule", resourceCulture);
        public static string Import => ResourceManager.GetString("Import", resourceCulture);
        public static string Open => ResourceManager.GetString("Open", resourceCulture);

        public static string ClassBreak => ResourceManager.GetString("ClassBreak", resourceCulture);
        public static string EnableClassBreak => ResourceManager.GetString("EnableClassBreak", resourceCulture);
        public static string EnableClassBreakDesc => ResourceManager.GetString("EnableClassBreakDesc", resourceCulture);
        public static string PreClassEnableTime => ResourceManager.GetString("PreClassEnableTime", resourceCulture);
        public static string PreClassEnableTimeDesc => ResourceManager.GetString("PreClassEnableTimeDesc", resourceCulture);
        public static string PostClassDisableDelay => ResourceManager.GetString("PostClassDisableDelay", resourceCulture);
        public static string PostClassDisableDelayDesc => ResourceManager.GetString("PostClassDisableDelayDesc", resourceCulture);

        public static string Verification => ResourceManager.GetString("Verification", resourceCulture);
        public static string EnableVerificationFlow => ResourceManager.GetString("EnableVerificationFlow", resourceCulture);
        public static string EnableVerificationFlowDesc => ResourceManager.GetString("EnableVerificationFlowDesc", resourceCulture);

        public static string PreClassReset => ResourceManager.GetString("PreClassReset", resourceCulture);
        public static string EnablePreClassReset => ResourceManager.GetString("EnablePreClassReset", resourceCulture);
        public static string EnablePreClassResetDesc => ResourceManager.GetString("EnablePreClassResetDesc", resourceCulture);
        public static string PreClassResetTime => ResourceManager.GetString("PreClassResetTime", resourceCulture);
        public static string PreClassResetTimeDesc => ResourceManager.GetString("PreClassResetTimeDesc", resourceCulture);

        public static string SubjectHistory => ResourceManager.GetString("SubjectHistory", resourceCulture);
        public static string EnableSubjectHistoryFilter => ResourceManager.GetString("EnableSubjectHistoryFilter", resourceCulture);
        public static string EnableSubjectHistoryFilterDesc => ResourceManager.GetString("EnableSubjectHistoryFilterDesc", resourceCulture);
        public static string SubjectHistoryBreakAssignment => ResourceManager.GetString("SubjectHistoryBreakAssignment", resourceCulture);
        public static string SubjectHistoryBreakAssignmentDesc => ResourceManager.GetString("SubjectHistoryBreakAssignmentDesc", resourceCulture);
        public static string BreakAssignmentOff => ResourceManager.GetString("BreakAssignmentOff", resourceCulture);
        public static string BreakAssignmentClass => ResourceManager.GetString("BreakAssignmentClass", resourceCulture);
        public static string BreakAssignmentDay => ResourceManager.GetString("BreakAssignmentDay", resourceCulture);

        public static string DataSource => ResourceManager.GetString("DataSource", resourceCulture);
        public static string DataSourceMode => ResourceManager.GetString("DataSourceMode", resourceCulture);
        public static string DataSourceModeDesc => ResourceManager.GetString("DataSourceModeDesc", resourceCulture);
        public static string DataSourceModeManual => ResourceManager.GetString("DataSourceModeManual", resourceCulture);
        public static string DataSourceModeClassIsland => ResourceManager.GetString("DataSourceModeClassIsland", resourceCulture);
        public static string DataSourceModeCses => ResourceManager.GetString("DataSourceModeCses", resourceCulture);

        public static string FloatingWindow => ResourceManager.GetString("FloatingWindow", resourceCulture);
        public static string HideFloatingWindowOnClassEnd => ResourceManager.GetString("HideFloatingWindowOnClassEnd", resourceCulture);
        public static string HideFloatingWindowOnClassEndDesc => ResourceManager.GetString("HideFloatingWindowOnClassEndDesc", resourceCulture);
    }
}

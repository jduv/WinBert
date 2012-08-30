namespace Arktos.WinBert.VsPackage
{
    using System;

    /// <summary>
    /// Static class that lists the GUID's used by the package.
    /// </summary>
    public static class GuidList
    {
        /// <summary>
        ///   The package guid string.
        /// </summary>
        public const string GuidWinBertVsPackagePkgString = "8640a686-935a-492c-a73f-a12329c3c3f2";

        /// <summary>
        ///   The package command set guid string.
        /// </summary>
        public const string GuidWinBertVsPackageCmdSetString = "e903d808-8b41-436a-9f45-1fdd3210056f";

        /// <summary>
        ///   The tool window persistence guid string.
        /// </summary>
        public const string GuidToolWindowPersistanceString = "b792c760-b860-4962-9692-bd415921e7c6";

        /// <summary>
        ///   The state of the UI where any solution has been loaded.
        /// </summary>
        public const string GuidUiContextAnySolution = "f1536ef8-92ec-443c-9ed7-fdadf150da82";

        /// <summary>
        ///   The package command set guid.
        /// </summary>
        public static readonly Guid GuidWinBertVsPackageCmdSet = new Guid(GuidWinBertVsPackageCmdSetString);
    }
}
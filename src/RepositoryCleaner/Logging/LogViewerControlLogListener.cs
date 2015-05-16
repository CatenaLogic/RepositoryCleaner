// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogViewerControlLogListener.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Logging
{
    using Catel.Logging;

    public class LogViewerControlLogListener : LogListenerBase
    {
        public LogViewerControlLogListener()
        {
            IgnoreCatelLogging = true;
        }
    }
}
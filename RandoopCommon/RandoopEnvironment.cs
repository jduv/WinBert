//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Apache License, Version 2.0.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************



using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// Global variables. 
    /// OMG a  CLASS for GLOBAL VARIABLES? You've GOT to be kidding me....
    /// </summary>
    public class RandoopEnvironment
    {
        private enum Configuration { Debug, Release }

        private readonly string internalErrorMessage = "RANDOOPBARE ABNORMAL TERMINATION";
        private readonly string invalidUserParameters = "RANDOOPBARE WAS GIVEN INVALID USER PARAMETERS";

        private static RandoopEnvironment instance;
        private static object padlock = new object();

        private IRandom randomizer;

        private string dHandler;
        private string miniDHandler;
        private string defaultDhi;

        public static RandoopEnvironment Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new RandoopEnvironment();
                            return instance;
                        }
                    }
                }

                return instance;
            }
        }

        public string RandoopBareInternalErrorMessage
        {
            get
            {
                return this.internalErrorMessage;
            }
        }

        public string RandoopBareInvalidUserParametersErrorMessage
        {
            get
            {
                return this.invalidUserParameters;
            }
        }

        public IRandom Randomizer
        {
            get 
            {
                return this.randomizer; 
            }

            set 
            { 
                this.randomizer = value; 
            }
        }

        public string MiniDHandler
        {
            get { return this.miniDHandler; }
        }

        public string DHandler
        {
            get { return this.dHandler; }
        }

        public string DefaultDhi
        {
            get { return this.defaultDhi; }
        }

        private RandoopEnvironment()
        {
            randomizer = new Common.SystemRandom();
        }

        /// <summary>
        /// Print failure message and terminate with error code 0.
        /// </summary>
        /// <param name="failureMessage"></param>
        public void Fail(String failureMessage)
        {
            Console.WriteLine("Randoop encountered a problem.");
            Console.WriteLine(failureMessage);
            System.Environment.Exit(1);
        }
    }
}

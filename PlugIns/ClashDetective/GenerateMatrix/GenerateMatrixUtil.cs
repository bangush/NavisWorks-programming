﻿//------------------------------------------------------------------
// NavisWorks Sample code
//------------------------------------------------------------------

// (C) Copyright 2013 by Autodesk Inc.

// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Navisworks.Api.Clash;
using Autodesk.Navisworks.Api;
using WF = System.Windows.Forms;

namespace ClashDetective
{
    /// <summary>
    /// Provides functions for creating, running and clearing ClashMatrix tests.
    /// </summary>
    public static class GenerateMatrixUtil
    {
        //Put in the name of tests generated by the plugin to show that they are affected by ClashMatrix commands. 
        public static readonly string TestIdentifierString = @"[CM]";
        ///<summary>
        /// Runs all clash tests generated by ClashMatrix i.e. all those where the DisplayName contains TestIdentifierString, skipping those marked
        /// complete if SkipComplete is true. Returns whether any tests were run.
        ///</summary>
        public static bool RunAllTests(bool SkipComplete)
        {
            Document theDocument = Application.MainDocument;
            DocumentClashTests DCT = theDocument.GetClash().TestsData;
            bool TestsExist = false, DidSomething = false;
            var i = 0;
            while (i < DCT.Tests.Count)
            {
                if (DCT.Tests[i].DisplayName.Contains(TestIdentifierString))
                {
                    TestsExist = true;
                    if (!((((ClashTest)DCT.Tests[i]).Status == ClashTestStatus.Complete) && SkipComplete))
                    {
                        DCT.TestsRunTest((ClashTest)DCT.Tests[i]);
                        DidSomething = true;
                    }

                }
                i++;
            }
            if (!TestsExist) WF.MessageBox.Show("No ClashMatrix tests exist for this model. Note that removing " + TestIdentifierString + " from the name of a generated test will prevent it from being affected by ClashMatrix commands.", "ClashMatrix");
            else if (!DidSomething) WF.MessageBox.Show("All ClashMatrix tests are up to date. To rerun tests regardless of status, hold CTRL and click \"Update all ClashMatrix tests\".", "ClashMatrix");

            return DidSomething;
        }

        ///<summary>
        /// Removes all clash tests previously generated by ClashMatrix i.e. all those where the DisplayName contains TestIdentifierString.
        ///</summary>
        public static void ClearGeneratedTests()
        {
            Document theDocument = Application.MainDocument;
            DocumentClash documentClash = theDocument.GetClash();
            var i = 0;
            while (i < documentClash.TestsData.Tests.Count)
            {
                if (documentClash.TestsData.Tests[i].DisplayName.Contains(TestIdentifierString)) documentClash.TestsData.TestsRemoveAt(i);
                else i++;
            }
        }

        ///<summary>
        /// Adds a new clash test of the given type for each pair of models in the document, using the default test settings.
        ///</summary>
        public static void GenerateTests()
        {
            GenerateTests(new ClashTest());
        }

        ///<summary>
        /// Adds a new clash test of the given type for each pair of models in the document. New tests are based on the template test given.
        ///</summary>
        public static void GenerateTests(ClashTest Template)
        {
            Document theDocument = Application.MainDocument;
            DocumentClashTests DCT = theDocument.GetClash().TestsData;

            LatestTestsStart = DCT.Tests.Count;
            for (int i = 0; i < theDocument.Models.Count(); i++)
            {
                Model model_a = theDocument.Models[i];
                for (int j = i + 1; j < theDocument.Models.Count(); j++)
                {
                    Model model_b = theDocument.Models[j];

                    ClashTest newTest = (ClashTest)Template.CreateCopyWithoutChildren();
                    newTest.DisplayName = TestIdentifierString + System.IO.Path.GetFileName(model_a.FileName) + " / " + System.IO.Path.GetFileName(model_b.FileName);

                    ModelItemCollection theCollection = new ModelItemCollection();
                    theCollection.Add(theDocument.Models[i].RootItem);
                    newTest.SelectionA.Selection.CopyFrom(theCollection);

                    theCollection = new ModelItemCollection();
                    theCollection.Add(theDocument.Models[j].RootItem);
                    newTest.SelectionB.Selection.CopyFrom(theCollection);

                    newTest.Guid = Guid.Empty; //The duplicated test has the same GUID as the original. This causes a new GUID to be generated.

                    DCT.TestsAddCopy(newTest);
                }
            }
            LatestTestsEnd = DCT.Tests.Count;
        }

        private static int LatestTestsStart; //Index of the first just-generated test.
        private static int LatestTestsEnd; //Index of the last just-generated test plus one.

        ///<summary>
        /// Runs the last set of tests generated. Only safe to run immediately after generating the tests.
        ///</summary>
        public static void RunLatestTests()
        {
            Document theDocument = Application.MainDocument;
            DocumentClashTests DCT = theDocument.GetClash().TestsData;

            for (var i = LatestTestsStart; i < LatestTestsEnd; i++)
            {
                DCT.TestsRunTest((ClashTest)DCT.Tests[i]);
            }
        }
    }
}

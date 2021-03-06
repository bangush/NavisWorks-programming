﻿//------------------------------------------------------------------
// NavisWorks Sample code
//------------------------------------------------------------------

// (C) Copyright 2014 by Autodesk Inc.

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

//.net using declaration
using System;
using System.Xml;

//Example using declaration
using Takeoff.XMLImportExport.Node;

namespace Takeoff.XMLImportExport.ImportCatalog
{
   public interface IXMLFileParser
   {
      TakeoffRootNode ParseCatalog();
      ItemGroupNode ParseItemGroup(XmlNode xmlNode);
      ItemNode ParseItem(XmlNode xmlNode);
      StepNode ParseStep(XmlNode xmlNode);
      StepResourceNode ParseStepResource(XmlNode xmlNode);
      ResourceGroupNode ParseResourceGroup(XmlNode xmlNode);
      ResourceNode ParseResource(XmlNode xmlNode);
      VariableCollectionNode ParseTakeoffVariableCollection(XmlNode xmlNode);
      VariableNode ParseTakeoffVariable(XmlNode xmlNode);
   }
}

﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.NodejsTools.Analysis.Analyzer;
using Microsoft.NodejsTools.Parsing;

namespace Microsoft.NodejsTools.Analysis.Values {
    /// <summary>
    /// A collection of references which are keyd off of project entry.
    /// </summary>
    [Serializable]
    class ReferenceDict : Dictionary<ProjectEntry, ReferenceList> {
        public ReferenceList GetReferences(ProjectEntry project) {
            ReferenceList builtinRef;
            if (!TryGetValue(project, out builtinRef) || builtinRef.Version != project.AnalysisVersion) {
                this[project] = builtinRef = new ReferenceList(project);
            }
            return builtinRef;
        }

        public IEnumerable<LocationInfo> AllReferences {
            get {
                foreach (var keyValue in this) {
                    if (keyValue.Value.References != null) {
                        foreach (var reference in keyValue.Value.References) {
                            yield return reference.GetLocationInfo(keyValue.Key);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// A list of references as stored for a single project entry.
    /// </summary>
    [Serializable]
    class ReferenceList : IReferenceable {
        public readonly int Version;
        public readonly ProjectEntry Project;
        public ISet<EncodedSpan> References;

        public ReferenceList(ProjectEntry project) {
            Version = project.AnalysisVersion;
            Project = project;
        }

        public void AddReference(EncodedSpan location) {
            HashSetExtensions.AddValue(ref References, location);
        }

        #region IReferenceable Members

        public IEnumerable<KeyValuePair<ProjectEntry, EncodedSpan>> Definitions {
            get { yield break; }
        }

        IEnumerable<KeyValuePair<ProjectEntry, EncodedSpan>> IReferenceable.References {
            get {
                if (References != null) {
                    foreach (var location in References) {
                        yield return new KeyValuePair<ProjectEntry, EncodedSpan>(Project, location);
                    }
                }
            }
        }

        #endregion
    }
}
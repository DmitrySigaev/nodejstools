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
using System.Linq;
using System.Text;

namespace Microsoft.NodejsTools.Analysis.Values {
    class ArgumentsValue : AnalysisValue {
        private readonly UserFunctionValue _function;
        public ArgumentsValue(UserFunctionValue function)
            : base(function.ProjectEntry) {
            _function = function;
        }

        public override IAnalysisSet GetIndex(Parsing.Node node, AnalysisUnit unit, IAnalysisSet index) {
            var res = AnalysisSet.Empty;
            if (_function._curArgs != null) {
                foreach (var value in index) {
                    var numIndex = value.Value.GetNumberValue();

                    if (numIndex != null &&
                        numIndex.Value >= 0 &&
                        (numIndex.Value % 1) == 0 &&    // integer number...
                        ((int)numIndex.Value) < _function._curArgs.Args.Length) {
                        res = res.Union(_function._curArgs.Args[(int)numIndex.Value]);
                    }
                }
            }
            return res;
        }
    }
}

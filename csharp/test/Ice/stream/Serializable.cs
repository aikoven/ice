//
// Copyright (c) ZeroC, Inc. All rights reserved.
//

using System;

namespace Ice.stream.Serialize
{

    [Serializable]
    public class Small // Fewer than 254 bytes with a BinaryFormatter.
    {
        public int i;
    }

}

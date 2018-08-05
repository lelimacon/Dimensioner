﻿using Dimensioner.Components.Elements;

namespace Dimensioner.Components.Tables
{
    public class TableGroupNode : GenericNode<TableGroupNode, TableGroupValue>
    {
    }

    public class TableGroupValue
    {
        public XbrlElement Element { get; set; }
        public Table Table { get; set; }
    }
}

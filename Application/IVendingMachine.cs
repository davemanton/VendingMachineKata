﻿using System.Collections.Generic;

namespace Application
{
    public interface IVendingMachine
    {
        string Display();
        void InsertCoin(string pieceOfMetal);
        ICollection<string> CheckRejectionBox();
    }
}
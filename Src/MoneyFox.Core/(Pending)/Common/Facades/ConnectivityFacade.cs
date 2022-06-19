﻿namespace MoneyFox.Core._Pending_.Common.Facades
{

    using Interfaces;

    public interface IConnectivityFacade
    {
        bool IsConnected { get; }
    }

    public class ConnectivityFacade : IConnectivityFacade
    {
        private readonly IConnectivityAdapter connectivityAdapter;

        public ConnectivityFacade(IConnectivityAdapter connectivityAdapter)
        {
            this.connectivityAdapter = connectivityAdapter;
        }

        public bool IsConnected => connectivityAdapter.IsConnected;
    }

}

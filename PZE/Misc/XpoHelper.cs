//Copyright © 2015 by Commend International GmbH.All rights reserved.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Affero General Public License, version 3,
//as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU Affero General Public License for more details.

//You should have received a copy of the GNU Affero General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.

namespace com.commend.tools.PZE.Misc
{
    using System.Configuration;
    using com.commend.tools.PZE.Data;
    using DevExpress.Xpo;
    using DevExpress.Xpo.DB;
    using DevExpress.Xpo.Metadata;

    public static class XpoHelper
    {
        public static Session GetNewSession()
        {
            return new Session(DataLayer);
        }

        public static UnitOfWork GetNewUnitOfWork()
        {
            return new UnitOfWork(DataLayer);
        }

        static volatile DataCacheNode fCacheNode;

        static DataCacheNode CacheNode
        {
            get
            {
                if (fCacheNode == null)
                {
                    lock (lockObject)
                    {
                        if (fCacheNode == null)
                        {
                            fCacheNode = GetCacheNode();
                        }
                    }
                }
                return fCacheNode;
            }
        }

        private readonly static object lockObject = new object();

        public static void NotifyDirtyTables(params string[] dirtyTablesNames)
        {
            CacheNode.NotifyDirtyTables(dirtyTablesNames);
        }

        public static void CatchUp()
        {
            CacheNode.CatchUp();
        }

        static volatile IDataLayer fDataLayer;

        static IDataLayer DataLayer
        {
            get
            {
                if (fDataLayer == null)
                {
                    IDataStore dataStore = CacheNode;
                    lock (lockObject)
                    {
                        if (fDataLayer == null)
                        {
                            fDataLayer = GetDataLayer(dataStore);
                        }
                    }
                }
                return fDataLayer;
            }
        }

        private static DataCacheNode GetCacheNode()
        {
            string conn = ConfigurationManager.ConnectionStrings["CommendPZE_DB"].ConnectionString;
            IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
            return new DataCacheNode(new DataCacheRoot(store));
        }

        private static IDataLayer GetDataLayer(IDataStore dataStore)
        {
            XpoDefault.Session = null;
            XPDictionary dict = new ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(User).Assembly);
            return new ThreadSafeDataLayer(dict, dataStore);
        }
    }
}
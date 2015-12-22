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

namespace com.commend.tools.PZE
{
    using System;
    using com.commend.tools.PZE.Misc;
    using com.commend.tools.PZE.View;

    public partial class ImportBMD : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BMDImporter importer = new BMDImporter();
            importer.Import();
        }
    }
}
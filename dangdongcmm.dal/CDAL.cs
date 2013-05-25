using System;
using System.Collections.Generic;
using System.Text;

using iSqlConnection = System.Data.IDbConnection;
using iSqlCommand = System.Data.IDbCommand;
using iSqlDataReader = System.Data.IDataReader;
using iSqlTransaction = System.Data.IDbTransaction;
using iSqlDataAdapter = System.Data.IDbDataAdapter;
using iCommandType = System.Data.CommandType;
using iSqlType = com.harmony.SqlHelper.SqlParameterDef.FieldStyle_T;
using iSqlParameter = com.harmony.SqlHelper.SqlParameterDef;

using com.harmony.SqlHelper;
using dangdongcmm.model;
using dangdongcmm.utilities;

namespace dangdongcmm.dal
{
    public class CAccesscounter
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "accesscounter";

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, counter, id)" +
            " VALUES(@NAME, @COUNTER, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, counter=@COUNTER WHERE id=@ID";
        private string SQL_GETIFO = "SELECT name, counter, id FROM " + Queryparam.Varstring.VAR_TABLENAME + " WHERE id<>0";

        public CAccesscounter()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_COUNTER = "@COUNTER";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_COUNTER, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, AccesscounterInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = info.Counter;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private AccesscounterInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                AccesscounterInfo info = new AccesscounterInfo();
                info.Name = dar.IsDBNull(++i) ? "" : dar.GetString(i);
                info.Counter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, AccesscounterInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IAccesscounter Members
        public bool Save(AccesscounterInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AccesscounterInfo Getinfo(string name)
        {
            try
            {
                AccesscounterInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND name='" + name + "'";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Updatecouter(string name)
        {
            try
            {
                int counter = 0;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND name='" + name + "'";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                        {
                            AccesscounterInfo info = this.getDataReader(dar);
                            counter = info.Counter + 1;
                            dar.Close();

                            SQL = "UPDATE " + TABLENAME + " SET counter=counter+1 WHERE name='" + name + "'";
                            HELPER.executeNonQuery(iConn, SQL);
                        }
                        else
                        {
                            AccesscounterInfo info = new AccesscounterInfo();
                            info.Name = name;
                            info.Counter = 1;
                            this.Save(info);

                            counter = 1;
                        }
                    }
                    iConn.Close();
                }
                return counter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatetotal()
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET counter=counter+1 WHERE id=1";
                    HELPER.executeNonQuery(iConn, SQL);

                    this.Updatetotal_detail(iConn, DateTime.Today.ToString("MM/dd/yyyy"));
                    this.Updatetotal_detail(iConn, CFunctions.FirstDayOfWeek(DateTime.Today) + " - " + CFunctions.LastDayOfWeek(DateTime.Today));
                    this.Updatetotal_detail(iConn, DateTime.Today.ToString("MM/yyyy"));

                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Updatetotal_detail(iSqlConnection iConn, string name)
        {
            try
            {
                string SQL = SQL_GETIFO;
                SQL += " AND name='" + name + "'";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        dar.Close();
                        SQL = "UPDATE " + TABLENAME + " SET counter=counter+1 WHERE name='" + name + "'";
                        HELPER.executeNonQuery(iConn, SQL);
                    }
                    else
                    {
                        AccesscounterInfo info = new AccesscounterInfo();
                        info.Name = name;
                        info.Counter = 1;
                        this.Save(info);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AccesscounterInfo Getinfototal()
        {
            try
            {
                AccesscounterInfo info = new AccesscounterInfo();
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND id=1";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                            info.Counter_Total = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                        dar.Close();
                    }

                    SQL = SQL_GETIFO + " AND name='" + DateTime.Today.AddDays(-1).ToString("MM/dd/yyyy") + "'";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                            info.Counter_Yesterday = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                        dar.Close();
                    }

                    SQL = SQL_GETIFO + " AND name='" + DateTime.Today.ToString("MM/dd/yyyy") + "'";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                            info.Counter_ThisDate = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                        dar.Close();
                    }

                    SQL = SQL_GETIFO + " AND name='" + CFunctions.FirstDayOfWeek(DateTime.Today) + " - " + CFunctions.LastDayOfWeek(DateTime.Today) + "'";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                            info.Counter_ThisWeek = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                        dar.Close();
                    }

                    SQL = SQL_GETIFO + " AND name='" + DateTime.Today.ToString("MM/yyyy") + "'";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                            info.Counter_ThisMonth = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                        dar.Close();
                    }

                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CCategory
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "category";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, note, url, filepreview, cid, orderd, status, markas, iconex, timeupdate, username, pis, pid, depth, customizeid, forsearch, id)" +
            " VALUES(@CODE, @NAME, @NOTE, @URL, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMEUPDATE, @USERNAME, @PIS, @PID, @DEPTH, @CUSTOMIZEID, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, note=@NOTE, url=@URL, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timeupdate=@TIMEUPDATE, username=@USERNAME, pis=@PIS, pid=@PID, depth=@DEPTH, customizeid=@CUSTOMIZEID, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.note, A.url, A.filepreview, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.customizeid, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_GETIFO2 = "SELECT A.code, A.name, A.note, A.url, A.filepreview, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.customizeid, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM vndd_category AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.note, A.url, A.filepreview, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.customizeid, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CCategory()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CCategory(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_NOTE = "@NOTE";
        private string PARM_URL = "@URL";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PIS = "@PIS";
        private string PARM_PID = "@PID";
        private string PARM_DEPTH = "@DEPTH";
        private string PARM_CUSTOMIZEID = "@CUSTOMIZEID";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_URL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PIS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_DEPTH, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CUSTOMIZEID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CategoryInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = CFunctions.SetDBString(info.Url);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = info.Iconex;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Pis;
                parms[++i].Value = info.Pid;
                parms[++i].Value = info.Depth;
                parms[++i].Value = info.Customizeid;
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Note);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CategoryInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CategoryInfo info = new CategoryInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Url = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Pis = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Pid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Depth = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Customizeid = dar.IsDBNull(++i) ? -1 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CategoryInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ICategory Members
        public bool Save(CategoryInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateAliasInfo(CategoryInfo info)
        {
            bool kq = false;
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE vndd_category SET alias = '";
                    SQL += CFunctions.install_urlname(info.Name).Replace(".aspx", "") + "'";
                    SQL += " Where id = " + info.Id;
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                kq = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return kq;
        }
        public CategoryInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                CategoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    
                    if(info != null)
                    {
                        CNews news = new CNews("vn");
                        info.dictCount = news.GetDictCount(iConn,info.Id.ToString());
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Get_setofname(string id)
        {
            try
            {
                string listname = "";
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT name FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND id IN(" + id + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            string name = dar.IsDBNull(0) ? string.Empty : dar.GetString(0);
                            listname += name + ", ";
                        }
                    }
                    iConn.Close();
                }
                return CFunctions.IsNullOrEmpty(listname) ? "" : listname.Trim().Remove(listname.Trim().Length - 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Get_setof(int pid, string listin)
        {
            try
            {
                string Setof_sub = this.Getsetof_sub(pid, listin) + pid.ToString();
                return Setof_sub;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string Getsetof_sub(int pid, string listin)
        {
            try
            {
                string list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT id, pis FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            int iid = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                            int pis = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                            if (pis != 0)
                                list = this.Getsetof_sub(iid, list);

                            list += iid + ",";
                        }
                        listin += list;
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryInfo> Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += pid == -1 ? "" : " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, cid, pid);
                    if(arr != null)
                    {
                        foreach (var cate in arr)
                        {
                            if (cate.Pid == 61)
                            {
                                CNews news = new CNews("vn");
                                cate.dictCount = news.GetDictCount(iConn, cate.Id.ToString());
                            }
                        } 
                    }
                    
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CategoryInfo GetCategoryInfo(string alias,int pid)
        {
            if (alias == "") return null;
            try
            {
                CategoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO2 + " and A.alias='" + alias + "'" + " and A.pid = " + pid;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = 1;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CategoryInfo GetCategoryInfo(string alias)
        {
            if (alias == "") return null;
            try
            {
                CategoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO2 + " and A.alias='" + alias + "'";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = 1;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int cid, int pid)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += pid == -1 ? "" : " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryInfo> Getlist_parent(int pid)
        {
            try
            {
                List<CategoryInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<CategoryInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryInfo> Getlist_sub(int pid, List<CategoryInfo> listin)
        {
            try
            {
                List<CategoryInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<CategoryInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<CategoryInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CategoryInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                CategoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryInfo> Wcmm_Getlist(int cid, int pid, ListOptions options)
        {
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    //string SQL = SQL_GETIFO;
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CategoryInfo> Wcmm_Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, cid, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int cid, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryInfo> Wcmm_Search(int cid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, cid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int cid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryInfo> Wcmm_Getlist_parent(int pid)
        {
            try
            {
                List<CategoryInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Wcmm_Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<CategoryInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryInfo> Wcmm_Getlist_sub(int pid, List<CategoryInfo> listin)
        {
            try
            {
                List<CategoryInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Wcmm_Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<CategoryInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<CategoryInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CategoryInfo> Wcmm_Getlist_ofiid(int iid, int belongto)
        {
            try
            {
                List<CategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id IN(SELECT categoryid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE iid=" + iid + " AND belongto=" + belongto + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CCategorytypeof
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "categorytypeof";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, note, status, markas, timeupdate, username, forsearch, id)" +
            " VALUES(@CODE, @NAME, @NOTE, @STATUS, @MARKAS, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, note=@NOTE, status=@STATUS, markas=@MARKAS, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.note, A.status, A.markas, A.timeupdate, A.username, A.id FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CCategorytypeof()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CCategorytypeof(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_NOTE = "@NOTE";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CategorytypeofInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.install_keyword(info.Name);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CategorytypeofInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CategorytypeofInfo info = new CategorytypeofInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CategorytypeofInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    //info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    //info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, string id)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(id)) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE id IN (" + id + ")";
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ICategorytypeof Members
        public bool Save(CategorytypeofInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(string iid)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(iid)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, iid);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CategorytypeofInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                CategorytypeofInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CategorytypeofInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                CategorytypeofInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategorytypeofInfo> Wcmm_Getlist(int status, int markas)
        {
            try
            {
                List<CategorytypeofInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += status == (int)CConstants.State.Status.None ? "" : " AND A.status=" + status;
                    SQL += markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + markas;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategorytypeofInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategorytypeofInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategorytypeofInfo> Wcmm_Search(int status, int markas, string keywords)
        {
            try
            {
                List<CategorytypeofInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += status == (int)CConstants.State.Status.None ? "" : " AND A.status=" + status;
                    SQL += markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + markas;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategorytypeofInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategorytypeofInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CCategoryattr
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "categoryattr";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, note, url, filepreview, cid, orderd, status, markas, iconex, timeupdate, username, pis, pid, depth, forsearch, id)" +
            " VALUES(@CODE, @NAME, @NOTE, @URL, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMEUPDATE, @USERNAME, @PIS, @PID, @DEPTH, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, note=@NOTE, url=@URL, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timeupdate=@TIMEUPDATE, username=@USERNAME, pis=@PIS, pid=@PID, depth=@DEPTH, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.note, A.url, A.filepreview, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.note, A.url, A.filepreview, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CCategoryattr(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_NOTE = "@NOTE";
        private string PARM_URL = "@URL";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PIS = "@PIS";
        private string PARM_PID = "@PID";
        private string PARM_DEPTH = "@DEPTH";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_URL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PIS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_DEPTH, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CategoryattrInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = CFunctions.SetDBString(info.Url);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = info.Iconex;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Pis;
                parms[++i].Value = info.Pid;
                parms[++i].Value = info.Depth;
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Note);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CategoryattrInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CategoryattrInfo info = new CategoryattrInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Url = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Pis = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Pid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Depth = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CategoryattrInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ICategoryattr Members
        public bool Save(CategoryattrInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CategoryattrInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                CategoryattrInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Get_setofname(string id)
        {
            try
            {
                string listname = "";
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT name FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND id IN(" + id + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            string name = dar.IsDBNull(0) ? string.Empty : dar.GetString(0);
                            listname += name + ", ";
                        }
                    }
                    iConn.Close();
                }
                return CFunctions.IsNullOrEmpty(listname) ? "" : listname.Trim().Remove(listname.Trim().Length - 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Get_setof(int pid, string listin)
        {
            try
            {
                string Setof_sub = this.Getsetof_sub(pid, listin) + pid.ToString();
                return Setof_sub;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string Getsetof_sub(int pid, string listin)
        {
            try
            {
                string list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT id, pis FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            int iid = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                            int pis = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                            if (pis != 0)
                                list = this.Getsetof_sub(iid, list);

                            list += iid + ",";
                        }
                        listin += list;
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryattrInfo> Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += pid == -1 ? "" : " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, cid, pid);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int cid, int pid)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += pid == -1 ? "" : " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryattrInfo> Getlist_parent(int pid)
        {
            try
            {
                List<CategoryattrInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<CategoryattrInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryattrInfo> Getlist_sub(int pid, List<CategoryattrInfo> listin)
        {
            try
            {
                List<CategoryattrInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<CategoryattrInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<CategoryattrInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CategoryattrInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                CategoryattrInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryattrInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryattrInfo> Wcmm_Getlist(int cid, int pid, ListOptions options)
        {
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    //string SQL = SQL_GETIFO;
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CategoryattrInfo> Wcmm_Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, cid, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int cid, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryattrInfo> Wcmm_Search(int cid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, cid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int cid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CategoryattrInfo> Wcmm_Getlist_parent(int pid)
        {
            try
            {
                List<CategoryattrInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Wcmm_Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<CategoryattrInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CategoryattrInfo> Wcmm_Getlist_sub(int pid, List<CategoryattrInfo> listin)
        {
            try
            {
                List<CategoryattrInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Wcmm_Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<CategoryattrInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<CategoryattrInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CategoryattrInfo> Wcmm_Getlist_ofiid(int iid, int belongto)
        {
            try
            {
                List<CategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id IN(SELECT categoryid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE iid=" + iid + " AND belongto=" + belongto + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CategoryattrInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CCart
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "cart";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, status, markas, timeupdate, timecomplete, lang, memberid, paymenttype, amount, shippingfee, discountfee, couponcode, checkout, note, shipping_name, shipping_address, shipping_address2, shipping_city, shipping_district, shipping_nationalid, shipping_zipcode, shipping_phone, shipping_email, billing_name, billing_address, billing_address2, billing_city, billing_district, billing_nationalid, billing_zipcode, billing_phone, billing_email, id)" +
            " VALUES(@NAME, @STATUS, @MARKAS, @TIMEUPDATE, @TIMECOMPLETE, @LANG, @MEMBERID, @PAYMENTTYPE, @AMOUNT, @SHIPPINGFEE, @DISCOUNTFEE, @COUPONCODE, @CHECKOUT, @NOTE, @SHIPPING_NAME, @SHIPPING_ADDRESS, @SHIPPING_ADDRESS2, @SHIPPING_CITY, @SHIPPING_DISTRICT, @SHIPPING_NATIONALID, @SHIPPING_ZIPCODE, @SHIPPING_PHONE, @SHIPPING_EMAIL, @BILLING_NAME, @BILLING_ADDRESS, @BILLING_ADDRESS2, @BILLING_CITY, @BILLING_DISTRICT, @BILLING_NATIONALID, @BILLING_ZIPCODE, @BILLING_PHONE, @BILLING_EMAIL, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, status=@STATUS, markas=@MARKAS, timeupdate=@TIMEUPDATE, timecomplete=@TIMECOMPLETE, lang=@LANG, memberid=@MEMBERID, paymenttype=@PAYMENTTYPE, amount=@AMOUNT, shippingfee=@SHIPPINGFEE, discountfee=@DISCOUNTFEE, couponcode=@COUPONCODE, checkout=@CHECKOUT, note=@NOTE, shipping_name=@SHIPPING_NAME, shipping_address=@SHIPPING_ADDRESS, shipping_address2=@SHIPPING_ADDRESS2, shipping_city=@SHIPPING_CITY, shipping_district=@SHIPPING_DISTRICT, shipping_nationalid=@SHIPPING_NATIONALID, shipping_zipcode=@SHIPPING_ZIPCODE, shipping_phone=@SHIPPING_PHONE, shipping_email=@SHIPPING_EMAIL, billing_name=@BILLING_NAME, billing_address=@BILLING_ADDRESS, billing_address2=@BILLING_ADDRESS2, billing_city=@BILLING_CITY, billing_district=@BILLING_DISTRICT, billing_nationalid=@BILLING_NATIONALID, billing_zipcode=@BILLING_ZIPCODE, billing_phone=@BILLING_PHONE, billing_email=@BILLING_EMAIL WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.status, A.markas, A.timeupdate, A.timecomplete, A.lang, A.memberid, M.username AS membername, A.paymenttype, A.amount, A.shippingfee, A.discountfee, A.couponcode, A.checkout, A.note, A.shipping_name, A.shipping_address, A.shipping_address2, A.shipping_city, A.shipping_district, A.shipping_nationalid, A.shipping_zipcode, A.shipping_phone, A.shipping_email, C1.name AS shipping_nationalname, A.billing_name, A.billing_address, A.billing_address2, A.billing_city, A.billing_district, A.billing_nationalid, A.billing_zipcode, A.billing_phone, A.billing_email, C2.name AS billing_nationalname, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.status, A.markas, A.timeupdate, A.timecomplete, A.lang, A.memberid, M.username AS membername, A.paymenttype, A.amount, A.shippingfee, A.discountfee, A.couponcode, A.checkout, A.note, A.shipping_name, A.shipping_address, A.shipping_address2, A.shipping_city, A.shipping_district, A.shipping_nationalid, A.shipping_zipcode, A.shipping_phone, A.shipping_email, C1.name AS shipping_nationalname, A.billing_name, A.billing_address, A.billing_address2, A.billing_city, A.billing_district, A.billing_nationalid, A.billing_zipcode, A.billing_phone, A.billing_email, C2.name AS billing_nationalname, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CCart(string lang)
        {
            HELPER = new SqlHelper();
            LANG = lang;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN ccs_member AS M ON M.id=A.memberid"
                + " LEFT JOIN " + LANG + "ccs_catalogue AS C1 ON C1.id=A.shipping_nationalid"
                + " LEFT JOIN " + LANG + "ccs_catalogue AS C2 ON C2.id=A.billing_nationalid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_TIMECOMPLETE = "@TIMECOMPLETE";
        private string PARM_LANG = "@LANG";
        private string PARM_MEMBERID = "@MEMBERID";
        private string PARM_PAYMENTTYPE = "@PAYMENTTYPE";
        private string PARM_AMOUNT = "@AMOUNT";
        private string PARM_SHIPPINGFEE = "@SHIPPINGFEE";
        private string PARM_DISCOUNTFEE = "@DISCOUNTFEE";
        private string PARM_COUPONCODE = "@COUPONCODE";
        private string PARM_CHECKOUT = "@CHECKOUT";
        private string PARM_NOTE = "@NOTE";
        private string PARM_SHIPPING_NAME = "@SHIPPING_NAME";
        private string PARM_SHIPPING_ADDRESS = "@SHIPPING_ADDRESS";
        private string PARM_SHIPPING_ADDRESS2 = "@SHIPPING_ADDRESS2";
        private string PARM_SHIPPING_CITY = "@SHIPPING_CITY";
        private string PARM_SHIPPING_DISTRICT = "@SHIPPING_DISTRICT";
        private string PARM_SHIPPING_NATIONALID = "@SHIPPING_NATIONALID";
        private string PARM_SHIPPING_ZIPCODE = "@SHIPPING_ZIPCODE";
        private string PARM_SHIPPING_PHONE = "@SHIPPING_PHONE";
        private string PARM_SHIPPING_EMAIL = "@SHIPPING_EMAIL";
        private string PARM_BILLING_NAME = "@BILLING_NAME";
        private string PARM_BILLING_ADDRESS = "@BILLING_ADDRESS";
        private string PARM_BILLING_ADDRESS2 = "@BILLING_ADDRESS2";
        private string PARM_BILLING_CITY = "@BILLING_CITY";
        private string PARM_BILLING_DISTRICT = "@BILLING_DISTRICT";
        private string PARM_BILLING_NATIONALID = "@BILLING_NATIONALID";
        private string PARM_BILLING_ZIPCODE = "@BILLING_ZIPCODE";
        private string PARM_BILLING_PHONE = "@BILLING_PHONE";
        private string PARM_BILLING_EMAIL = "@BILLING_EMAIL";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMECOMPLETE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_LANG, iSqlType.Field_tString),
					    new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_PAYMENTTYPE, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_AMOUNT, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_SHIPPINGFEE, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_DISCOUNTFEE, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_COUPONCODE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_CHECKOUT, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_ADDRESS2, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_CITY, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_DISTRICT, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_NATIONALID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_SHIPPING_ZIPCODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SHIPPING_PHONE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_SHIPPING_EMAIL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_ADDRESS2, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_CITY, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_DISTRICT, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_NATIONALID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_BILLING_ZIPCODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BILLING_PHONE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_BILLING_EMAIL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CartInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name) + info.Id.ToString();
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecomplete);
                parms[++i].Value = CFunctions.SetDBString(info.Lang);
                parms[++i].Value = info.Memberid;
                parms[++i].Value = info.Paymenttype;
                parms[++i].Value = info.Amount;
                parms[++i].Value = info.Shippingfee;
                parms[++i].Value = info.Discountfee;
                parms[++i].Value = CFunctions.SetDBString(info.Couponcode);
                parms[++i].Value = info.Checkout;
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Name);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Address);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Address2);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_City);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_District);
                parms[++i].Value = info.Shipping_Nationalid;
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Zipcode);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Phone);
                parms[++i].Value = CFunctions.SetDBString(info.Shipping_Email);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Name);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Address);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Address2);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_City);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_District);
                parms[++i].Value = info.Billing_Nationalid;
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Zipcode);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Phone);
                parms[++i].Value = CFunctions.SetDBString(info.Billing_Email);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CartInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CartInfo info = new CartInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timecomplete = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Lang = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Memberid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Membername = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Paymenttype = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Amount = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i));
                info.Shippingfee = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i));
                info.Discountfee = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i));
                info.Couponcode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Checkout = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Address2 = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_City = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_District = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Nationalid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Shipping_Zipcode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Phone = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Shipping_Nationalname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Address2 = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_City = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_District = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Nationalid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Billing_Zipcode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Phone = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Billing_Nationalname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CartInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ICart Members
        public CartInfo GetinfofullNotcheckout(int memberid)
        {
            if (memberid == 0) return null;
            try
            {
                CartInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.memberid=@MEMBERID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.checkout=0";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                            info.lCartitem = (new CCartitem(LANG)).Getlistfull(info.Id);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CartInfo> GetinfofullCheckout(int memberid, ListOptions options)
        {
            if (memberid == 0) return null;
            try
            {
                List<CartInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.memberid=@MEMBERID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.checkout=1";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        while (dar.Read())
                        {
                            CartInfo info = this.getDataReader(dar);
                            info.lCartitem = (new CCartitem(LANG)).Getlistfull(info.Id);
                            if (arr == null)
                                arr = new List<CartInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Save(CartInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                                (new CCartitem(LANG)).Save(trans, info.Id, info.lCartitem);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CartInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                CartInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CartInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<CartInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CartInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CartInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CartInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<CartInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CartInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CartInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CCartitem
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "cartitem";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(cartid, productid, quantity, amount, temporaryid, id)" +
            " VALUES(@CARTID, @PRODUCTID, @QUANTITY, @AMOUNT, @TEMPORARYID, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET cartid=@CARTID, productid=@PRODUCTID, quantity=@QUANTITY, amount=@AMOUNT, temporaryid=@TEMPORARYID WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.cartid, A.productid, P.name AS productname, A.quantity, A.amount, A.temporaryid, A.id FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0";

        public CCartitem(string lang)
        {
            HELPER = new SqlHelper();
            LANG = lang;

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + "ccs_product AS P ON P.id=A.productid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CARTID = "@CARTID";
        private string PARM_PRODUCTID = "@PRODUCTID";
        private string PARM_QUANTITY = "@QUANTITY";
        private string PARM_AMOUNT = "@AMOUNT";
        private string PARM_TEMPORARYID = "@TEMPORARYID";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CARTID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PRODUCTID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_QUANTITY, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_AMOUNT, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_TEMPORARYID, iSqlType.Field_tDouble),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CartitemInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.CartID;
                parms[++i].Value = info.ProductID;
                parms[++i].Value = info.Quantity;
                parms[++i].Value = info.Amount;
                parms[++i].Value = info.TemporaryID;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CartitemInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CartitemInfo info = new CartitemInfo();
                info.CartID = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.ProductID = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Productname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Quantity = dar.IsDBNull(++i) ? -1 : dar.GetInt32(i);
                info.Amount = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i));
                info.TemporaryID = dar.IsDBNull(++i) ? 0 : long.Parse(dar.GetValue(i).ToString());
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CartitemInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, int cartid)
        {
            try
            {
                if (trans == null) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE cartid=@CARTID";
                iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CARTID, iSqlType.Field_tInterger),
                    };
                int i = -1;
                parms[++i].Value = cartid;
                HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitemone(iSqlTransaction trans, int id)
        {
            try
            {
                if (trans == null) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE id=@ID";
                iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                int i = -1;
                parms[++i].Value = id;
                HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ICartitem Members
        public bool Save(int cartid, List<CartitemInfo> list)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            foreach (CartitemInfo info in list)
                            {
                                info.CartID = cartid;
                                this.Saveitem(trans, info);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int cartid)
        {
            if (cartid == 0) return false;
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, cartid);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Deleteone(int id)
        {
            if (id == 0) return false;
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitemone(trans, id);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Save(iSqlTransaction trans, int cartid, List<CartitemInfo> list)
        {
            try
            {
                try
                {
                    foreach (CartitemInfo info in list)
                    {
                        info.CartID = cartid;
                        this.Saveitem(trans, info);
                    }

                    //trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CartitemInfo> Getlistfull(int cartid)
        {
            try
            {
                List<CartitemInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.cartid=@CARTID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CARTID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = cartid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        CProduct DALProduct = new CProduct(LANG);
                        while (dar.Read())
                        {
                            CartitemInfo info = this.getDataReader(dar);
                            info.iProduct = DALProduct.Getinfo(info.ProductID);
                            
                            if (arr == null)
                                arr = new List<CartitemInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CartitemInfo> Wcmm_Getlist(int cartid)
        {
            try
            {
                List<CartitemInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.cartid=" + cartid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CartitemInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CartitemInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CComment
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "comment";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, introduce, description, orderd, status, markas, iconex, timecreate, timeupdate, username, iid, belongto, viewcounter, rating, sender_name, sender_address, sender_email, sender_phone, forsearch, id)" +
            " VALUES(@NAME, @INTRODUCE, @DESCRIPTION, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMECREATE, @TIMEUPDATE, @USERNAME, @IID, @BELONGTO, @VIEWCOUNTER, @RATING, @SENDER_NAME, @SENDER_ADDRESS, @SENDER_EMAIL, @SENDER_PHONE, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, introduce=@INTRODUCE, description=@DESCRIPTION, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timecreate=@TIMECREATE, timeupdate=@TIMEUPDATE, username=@USERNAME, iid=@IID, belongto=@BELONGTO, viewcounter=@VIEWCOUNTER, rating=@RATING, sender_name=@SENDER_NAME, sender_address=@SENDER_ADDRESS, sender_email=@SENDER_EMAIL, sender_phone=@SENDER_PHONE, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.introduce, A.description, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.iid, A.belongto, A.viewcounter, A.rating, A.sender_name, A.sender_address, A.sender_email, A.sender_phone, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.introduce, A.description, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.iid, A.belongto, A.viewcounter, A.rating, A.sender_name, A.sender_address, A.sender_email, A.sender_phone, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CComment(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_INTRODUCE = "@INTRODUCE";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMECREATE = "@TIMECREATE";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_IID = "@IID";
        private string PARM_BELONGTO = "@BELONGTO";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_RATING = "@RATING";
        private string PARM_SENDER_NAME = "@SENDER_NAME";
        private string PARM_SENDER_ADDRESS = "@SENDER_ADDRESS";
        private string PARM_SENDER_EMAIL = "@SENDER_EMAIL";
        private string PARM_SENDER_PHONE = "@SENDER_PHONE";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_INTRODUCE, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),							
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMECREATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_RATING, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_SENDER_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_EMAIL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_PHONE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, CommentInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Introduce);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecreate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Iid;
                parms[++i].Value = info.Belongto;
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = info.Rating;
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Name);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Address);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Email);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Phone);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Introduce) + " " + CFunctions.install_keyword(info.Description) + " " + CFunctions.install_keyword(info.Sender_Name) + " " + CFunctions.install_keyword(info.Sender_Address) + " " + CFunctions.install_keyword(info.Sender_Email) + " " + CFunctions.install_keyword(info.Sender_Phone);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CommentInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                CommentInfo info = new CommentInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timecreate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Iid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Belongto = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rating = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Sender_Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Phone = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, CommentInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region IComment Members
        public bool Save(CommentInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CommentInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                CommentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CommentInfo> Getlist(int belongto, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += belongto == 0 ? "" : " AND A.belongto=" + belongto;
                    SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, belongto, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int belongto, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += belongto == 0 ? "" : " AND A.belongto=" + belongto;
                SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CommentInfo> Search(int belongto, int pid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += " AND A.belongto=" + belongto;
                    SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, belongto, pid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, int belongto, int pid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += " AND A.belongto=" + belongto;
                SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CommentInfo> Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public CommentInfo Getinforating(int belongto, int pid)
        {
            if (belongto == 0 || pid == 0) return null;
            try
            {
                CommentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT COUNT(id), SUM(rating) FROM " + TABLENAME + " AS A WHERE id<>0";
                    SQL += " AND A.iid=@IID";
                    SQL += " AND A.belongto=@BELONGTO";
                    SQL += " AND A.status=" + (int)CConstants.State.Status.Actived;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = pid;
                    parms[++i].Value = belongto;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            i = -1;
                            info = new CommentInfo();
                            info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                            info.Rating = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Checkdupemail(int belongto, int pid, string email)
        {
            try
            {
                bool vlreturn = false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT A.id FROM " + TABLENAME + " AS A WHERE A.id<>0"
                        + " AND A.belongto=@BELONGTO AND A.iid=@IID AND A.sender_email=@SENDER_EMAIL";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_SENDER_EMAIL, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = belongto;
                    parms[++i].Value = pid;
                    parms[++i].Value = email;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            vlreturn = true;
                        }
                    }
                    iConn.Close();
                }
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Checkdupphone(int belongto, int pid, string phone)
        {
            try
            {
                bool vlreturn = false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT A.id FROM " + TABLENAME + " AS A WHERE A.id<>0"
                        + " AND A.belongto=@BELONGTO AND A.iid=@IID AND A.sender_phone=@SENDER_PHONE";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_SENDER_PHONE, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = belongto;
                    parms[++i].Value = pid;
                    parms[++i].Value = phone;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            vlreturn = true;
                        }
                    }
                    iConn.Close();
                }
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public CommentInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                CommentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CommentInfo> Wcmm_Getlist(string iid)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CommentInfo> Wcmm_Getlist(int belongto, int pid)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.belongto=" + belongto;
                    SQL += pid == 0 ? "" : " AND A.iid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CommentInfo> Wcmm_Getlist(int belongto, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    //SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += belongto == 0 ? "" : " AND A.belongto=" + belongto;
                    SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, belongto, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int belongto, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                //SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += belongto == 0 ? "" : " AND A.belongto=" + belongto;
                SQL += pid == 0 ? "" : " AND A.iid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<CommentInfo> Wcmm_Search(int belongto, int pid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<CommentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    //SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.belongto=" + belongto;
                    SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            CommentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<CommentInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Searchcount(iConn, belongto, pid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int belongto, int pid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                //SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += " AND A.belongto=" + belongto;
                SQL += pid == 0 ? "" : " AND A.iid=" + pid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMeCreditcard
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "mecreditcard";
        //private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(firstname, lastname, cardnumber, cardtype, expirationmonth, expirationyear, address, city, state, zipcode, memberid, balance, status, id)" +
            " VALUES(@FIRSTNAME, @LASTNAME, @CARDNUMBER, @CARDTYPE, @EXPIRATIONMONTH, @EXPIRATIONYEAR, @ADDRESS, @CITY, @STATE, @ZIPCODE, @MEMBERID, @BALANCE, @STATUS, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET firstname=@FIRSTNAME, lastname=@LASTNAME, cardnumber=@CARDNUMBER, cardtype=@CARDTYPE, expirationmonth=@EXPIRATIONMONTH, expirationyear=@EXPIRATIONYEAR, address=@ADDRESS, city=@CITY, state=@STATE, zipcode=@ZIPCODE, memberid=@MEMBERID, balance=@BALANCE, status=@STATUS WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.firstname, A.lastname, A.cardnumber, A.cardtype, A.expirationmonth, A.expirationyear, A.address, A.city, A.state, A.zipcode, A.memberid, A.balance, A.status, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.firstname, A.lastname, A.cardnumber, A.cardtype, A.expirationmonth, A.expirationyear, A.address, A.city, A.state, A.zipcode, A.memberid, A.status, A.balance, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CMeCreditcard()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_FIRSTNAME = "@FIRSTNAME";
        private string PARM_LASTNAME = "@LASTNAME";
        private string PARM_CARDNUMBER = "@CARDNUMBER";
        private string PARM_CARDTYPE = "@CARDTYPE";
        private string PARM_EXPIRATIONMONTH = "@EXPIRATIONMONTH";
        private string PARM_EXPIRATIONYEAR = "@EXPIRATIONYEAR";
        private string PARM_ADDRESS = "@ADDRESS";
        private string PARM_CITY = "@CITY";
        private string PARM_STATE = "@STATE";
        private string PARM_ZIPCODE = "@ZIPCODE";
        private string PARM_MEMBERID = "@MEMBERID";
        private string PARM_BALANCE = "@BALANCE";
        private string PARM_STATUS = "@STATUS";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_FIRSTNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_LASTNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CARDNUMBER, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CARDTYPE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_EXPIRATIONMONTH, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_EXPIRATIONYEAR, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CITY, iSqlType.Field_tString),
					    new iSqlParameter(PARM_STATE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ZIPCODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_BALANCE, iSqlType.Field_tReal),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MeCreditcardInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Firstname);
                parms[++i].Value = CFunctions.SetDBString(info.Lastname);
                parms[++i].Value = CFunctions.SetDBString(info.Cardnumber);
                parms[++i].Value = CFunctions.SetDBString(info.Cardtype);
                parms[++i].Value = info.Expirationmonth;
                parms[++i].Value = info.Expirationyear;
                parms[++i].Value = CFunctions.SetDBString(info.Address);
                parms[++i].Value = CFunctions.SetDBString(info.City);
                parms[++i].Value = CFunctions.SetDBString(info.State);
                parms[++i].Value = CFunctions.SetDBString(info.Zipcode);
                parms[++i].Value = info.Memberid;
                parms[++i].Value = info.Balance;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MeCreditcardInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MeCreditcardInfo info = new MeCreditcardInfo();
                info.Firstname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Lastname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cardnumber = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cardtype = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Expirationmonth = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Expirationyear = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.City = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.State = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Zipcode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Memberid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Balance = dar.IsDBNull(++i) ? 0 : dar.GetFloat(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MeCreditcardInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, string id)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(id)) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE id IN (" + id + ")";
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IMeCreditcard Members
        public bool Save(MeCreditcardInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MeCreditcardInfo Getinfo(int memberid)
        {
            if (memberid == 0) return null;
            try
            {
                MeCreditcardInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.memberid=@MEMBERID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(string id)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, id);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MeCreditcardInfo Wcmm_Getinfo(int memberid)
        {
            if (memberid == 0) return null;
            try
            {
                MeCreditcardInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.memberid=@MEMBERID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMeCreditcardHistory
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "mecreditcardhistory";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(memberid, creditcardid, balance, amount, addorsub, note, status, timeupdate, forsearch, id)" +
            " VALUES(@MEMBERID, @CREDITCARDID, @BALANCE, @AMOUNT, @ADDORSUB, @NOTE, @STATUS, @TIMEUPDATE, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET memberid=@MEMBERID, creditcardid=@CREDITCARDID, balance=@BALANCE, amount=@AMOUNT, addorsub=@ADDORSUB, note=@NOTE, status=@STATUS, timeupdate=@TIMEUPDATE, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.memberid, A.creditcardid, A.balance, A.amount, A.addorsub, A.note, A.status, A.timeupdate, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0";
        private string SQL_GETIFOPAGING = "SELECT A.memberid, A.creditcardid, A.balance, A.amount, A.addorsub, A.note, A.status, A.timeupdate, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0";
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0";

        public CMeCreditcardHistory()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CMeCreditcardHistory(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_MEMBERID = "@MEMBERID";
        private string PARM_CREDITCARDID = "@CREDITCARDID";
        private string PARM_BALANCE = "@BALANCE";
        private string PARM_AMOUNT = "@AMOUNT";
        private string PARM_ADDORSUB = "@ADDORSUB";
        private string PARM_NOTE = "@NOTE";
        private string PARM_STATUS = "@STATUS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
					    new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CREDITCARDID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_BALANCE, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_AMOUNT, iSqlType.Field_tReal),
					    new iSqlParameter(PARM_ADDORSUB, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MeCreditcardHistoryInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.Memberid;
                parms[++i].Value = info.Creditcardid;
                parms[++i].Value = info.Balance;
                parms[++i].Value = info.Amount;
                parms[++i].Value = info.Addorsub;
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = info.Status;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.install_keyword(info.iMember.Username) + " " + CFunctions.install_keyword(info.iMember.Fullname);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MeCreditcardHistoryInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MeCreditcardHistoryInfo info = new MeCreditcardHistoryInfo();
                info.Memberid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Creditcardid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Balance = dar.IsDBNull(++i) ? 0 : dar.GetFloat(i);
                info.Amount = dar.IsDBNull(++i) ? 0 : dar.GetFloat(i);
                info.Addorsub = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MeCreditcardHistoryInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IMeCreditcardHistory Members
        public bool Save(MeCreditcardHistoryInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MeCreditcardHistoryInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MeCreditcardHistoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MeCreditcardHistoryInfo Getinfolastest(int memberid)
        {
            if (memberid == 0) return null;
            try
            {
                MeCreditcardHistoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort("id", "DESC"));
                    SQL += " AND A.memberid=@MEMBERID";
                    SQL = "SELECT TOP 1 * FROM(" + SQL + ") AS T WHERE id<>0";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MeCreditcardHistoryInfo> Getlist(int memberid, ListOptions options, out int numResults)
        {
            try
            {
                List<MeCreditcardHistoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.memberid=@MEMBERID";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_MEMBERID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = memberid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        while (dar.Read())
                        {
                            MeCreditcardHistoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MeCreditcardHistoryInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, memberid);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int memberid)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += memberid == 0 ? "" : " AND A.memberid=" + memberid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public MeCreditcardHistoryInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MeCreditcardHistoryInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MeCreditcardHistoryInfo> Wcmm_Getlist(int memberid, ListOptions options, out int numResults)
        {
            try
            {
                List<MeCreditcardHistoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += memberid == 0 ? "" : " AND A.memberid=" + memberid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        CMember DALMem = new CMember(LANG);
                        while (dar.Read())
                        {
                            MeCreditcardHistoryInfo info = this.getDataReader(dar);
                            info.iMember = DALMem.Getinfo(info.Memberid);
                            if (arr == null)
                                arr = new List<MeCreditcardHistoryInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, memberid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int memberid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += memberid == 0 ? "" : " AND A.memberid=" + memberid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<MeCreditcardHistoryInfo> Wcmm_Search(int memberid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<MeCreditcardHistoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += memberid == 0 ? "" : " AND A.memberid=" + memberid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        CMember DALMem = new CMember(LANG);
                        while (dar.Read())
                        {
                            MeCreditcardHistoryInfo info = this.getDataReader(dar);
                            info.iMember = DALMem.Getinfo(info.Memberid);
                            if (arr == null)
                                arr = new List<MeCreditcardHistoryInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, memberid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int memberid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += memberid == 0 ? "" : " AND A.memberid=" + memberid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        
        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CFeedback
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "feedback";

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, description, sender_name, sender_address, sender_email, sender_phone, status, markas, timeupdate, pis, pid, depth, viewcounter, forsearch, id)" +
            " VALUES(@NAME, @DESCRIPTION, @SENDER_NAME, @SENDER_ADDRESS, @SENDER_EMAIL, @SENDER_PHONE, @STATUS, @MARKAS, @TIMEUPDATE, @PIS, @PID, @DEPTH, @VIEWCOUNTER, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, description=@DESCRIPTION, sender_name=@SENDER_NAME, sender_address=@SENDER_ADDRESS, sender_email=@SENDER_EMAIL, sender_phone=@SENDER_PHONE, status=@STATUS, markas=@MARKAS, timeupdate=@TIMEUPDATE, pis=@PIS, pid=@PID, depth=@DEPTH, viewcounter=@VIEWCOUNTER, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.description, A.sender_name, A.sender_address, A.sender_email, A.sender_phone, A.status, A.markas, A.timeupdate, A.pis, A.pid, A.depth, A.viewcounter, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.description, A.sender_name, A.sender_address, A.sender_email, A.sender_phone, A.status, A.markas, A.timeupdate, A.pis, A.pid, A.depth, A.viewcounter, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CFeedback()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_SENDER_NAME = "@SENDER_NAME";
        private string PARM_SENDER_ADDRESS = "@SENDER_ADDRESS";
        private string PARM_SENDER_EMAIL = "@SENDER_EMAIL";
        private string PARM_SENDER_PHONE = "@SENDER_PHONE";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_PIS = "@PIS";
        private string PARM_PID = "@PID";
        private string PARM_DEPTH = "@DEPTH";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),
					    new iSqlParameter(PARM_SENDER_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_EMAIL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SENDER_PHONE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_PIS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_DEPTH, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, FeedbackInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Name);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Address);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Email);
                parms[++i].Value = CFunctions.SetDBString(info.Sender_Phone);
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = info.Pis;
                parms[++i].Value = info.Pid;
                parms[++i].Value = info.Depth;
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = CFunctions.install_keyword(info.Sender_Name) + " " + CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Description);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private FeedbackInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                FeedbackInfo info = new FeedbackInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sender_Phone = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Pis = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Pid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Depth = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, FeedbackInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleterecursive(iSqlTransaction trans, string iid)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(iid)) return false;

                string SQL_SELECT = "SELECT id FROM " + TABLENAME + " WHERE pid IN (" + iid + ")";
                string strIid = "";
                using (iSqlDataReader dar = HELPER.executeReader(trans, SQL_SELECT))
                {
                    while (dar.Read())
                    {
                        int id = dar.IsDBNull(0) ? -1 : dar.GetInt32(0);
                        strIid += id + ",";
                    }
                    if (!string.IsNullOrEmpty(strIid))
                        strIid = strIid.Remove(strIid.LastIndexOf(","), 1);
                }
                this.Deleterecursive(trans, strIid);

                string SQL_DELETE = "DELETE FROM " + TABLENAME + " WHERE id IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL_DELETE);

                SQL_DELETE = "DELETE FROM " + TABLENAME + " WHERE pid IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL_DELETE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region IFeedback Members
        public bool Save(FeedbackInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(string iid)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(iid)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleterecursive(trans, iid);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FeedbackInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                FeedbackInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Getlist_parent(int pid)
        {
            try
            {
                List<FeedbackInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<FeedbackInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Getlist_sub(int pid, List<FeedbackInfo> listin)
        {
            try
            {
                List<FeedbackInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<FeedbackInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<FeedbackInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Getlist(int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Getlistcount(iConn, pid, options.Markas);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int pid, int markas)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + markas;
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<FeedbackInfo> Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Searchcount(iConn, keywords, options.Markas);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string keywords, int markas)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + markas;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public FeedbackInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                FeedbackInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Wcmm_Getlist(int pid)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FeedbackInfo> Wcmm_Getlist(int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.pid=" + pid;
                    SQL += " AND A.pis=0";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, pid);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int pid)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<FeedbackInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    //SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<FeedbackInfo> Wcmm_Getlistfolder(int pid)
        {
            try
            {
                List<FeedbackInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;
                    SQL += " AND A.pis>0";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FeedbackInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FeedbackInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Countnew()
        {
            try
            {
                int counter = 0;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                    SQL += " AND A.pis=0";
                    SQL += " AND A.viewcounter=0";
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                        {
                            counter = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                        }
                    }
                    iConn.Close();
                }
                return counter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatepis(string id, int isAdd, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    //string SQL1 = "SELECT pid FROM " + TABLENAME + " WHERE id<>0 AND id IN(" + id + ")";
                    string valueset = value.ToString();
                    if (isAdd == CConstants.NUM_INCREASE)
                        valueset = "pis+" + valueset;
                    else if (isAdd == CConstants.NUM_DECREASE)
                        valueset = "pis-" + valueset;
                    string SQL2 = "UPDATE " + TABLENAME + " SET pis=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL2);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CFileattach
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "fileattach";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, path, note, subject, url, sized, typed, orderd, status, markas, iconex, timeupdate, username, belongto, iid, id)" +
            " VALUES(@NAME, @PATH, @NOTE, @SUBJECT, @URL, @SIZED, @TYPED, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMEUPDATE, @USERNAME, @BELONGTO, @IID, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, path=@PATH, note=@NOTE, subject=@SUBJECT, url=@URL, sized=@SIZED, typed=@TYPED, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timeupdate=@TIMEUPDATE, username=@USERNAME, belongto=@BELONGTO, iid=@IID WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.path, A.note, A.subject, A.url, A.sized, A.typed, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.belongto, A.iid, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.path, A.note, A.subject, A.url, A.sized, A.typed, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.belongto, A.iid, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CFileattach()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CFileattach(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_PATH = "@PATH";
        private string PARM_NOTE = "@NOTE";
        private string PARM_SUBJECT = "@SUBJECT";
        private string PARM_URL = "@URL";
        private string PARM_SIZED = "@SIZED";
        private string PARM_TYPED = "@TYPED";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_BELONGTO = "@BELONGTO";
        private string PARM_IID = "@IID";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PATH, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SUBJECT, iSqlType.Field_tString),
					    new iSqlParameter(PARM_URL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SIZED, iSqlType.Field_tBigInt),
					    new iSqlParameter(PARM_TYPED, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, FileattachInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Path);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = CFunctions.SetDBString(info.Subject);
                parms[++i].Value = CFunctions.SetDBString(info.Url);
                parms[++i].Value = info.Sized;
                parms[++i].Value = info.Typed;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Belongto;
                parms[++i].Value = info.Iid;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private FileattachInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                FileattachInfo info = new FileattachInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Path = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Subject = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Url = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sized = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);
                info.Typed = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Belongto = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Iid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, FileattachInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, string id)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(id)) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE id IN (" + id + ")";
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, int belongto, string iid)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(iid)) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE iid IN (" + iid + ") AND belongto=" + belongto;
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IFileattach Members
        public bool Save(FileattachInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Save(List<FileattachInfo> list)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            foreach (FileattachInfo info in list)
                                this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Save(List<FileattachInfo> list, int iid)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            foreach (FileattachInfo info in list)
                            {
                                info.Iid = iid;
                                this.Saveitem(trans, info);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public FileattachInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                FileattachInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FileattachInfo> Getlist(int belongto, int iid, ListOptions options, out int numResults)
        {
            try
            {
                List<FileattachInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += " AND A.belongto=" + belongto;
                    SQL += " AND A.iid=" + iid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FileattachInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FileattachInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, belongto, iid, options.Markas);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int belongto, int iid, int markas)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + markas;
                SQL += " AND A.belongto=" + belongto;
                SQL += " AND A.iid=" + iid;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public bool Delete(string id)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, id);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(int belongto, string iid)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(iid)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, belongto, iid);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FileattachInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                FileattachInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<FileattachInfo> Wcmm_Getlist(int belongto, int iid)
        {
            try
            {
                List<FileattachInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.belongto=" + belongto;
                    SQL += " AND A.iid=" + iid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            FileattachInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<FileattachInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CGeneral
    {
        private SqlHelper HELPER = null;
        private string TABLENAME;
        private string LANG;

        private string SQL_GETIFO = "SELECT A.code, A.name, A.introduce, A.description, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.viewcounter, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A "
            + Queryparam.Varstring.VAR_JOINEXPRESSION
            + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_GETIFO_COM = "SELECT A.name, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.introduce, A.description, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.viewcounter, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A "
            + Queryparam.Varstring.VAR_JOINEXPRESSION
            + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING_COM = "SELECT A.name, A.status, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT_COM = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING_SEARCH = "SELECT A.code, A.name, A.introduce, A.description, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A "
            + Queryparam.Varstring.VAR_JOINEXPRESSION
            + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT_SEARCH = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CGeneral()
        {
            HELPER = new SqlHelper();

            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO_COM = SQL_GETIFO_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING_COM = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING_SEARCH = SQL_GETIFOPAGING_SEARCH.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CGeneral(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            
            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFO_COM = SQL_GETIFO_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING_COM = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING_SEARCH = SQL_GETIFOPAGING_SEARCH.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }
        public CGeneral(string lang, string tablename)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + tablename;
            
            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFO_COM = SQL_GETIFO_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING_COM = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING_SEARCH = SQL_GETIFOPAGING_SEARCH.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }
        public CGeneral(string lang, int belongto)
        {
            HELPER = new SqlHelper();

            string tablename = CFunctions.Get_Definecatrelate(belongto, Queryparam.Defstring.Table);
            LANG = lang;
            TABLENAME = lang + tablename;
            
            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFO_COM = SQL_GETIFO_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING_COM = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING_SEARCH = SQL_GETIFOPAGING_SEARCH.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }


        #region parameters
        private string PARM_ID = "@ID";
        #endregion

        #region private methods
        private GeneralInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                GeneralInfo info = new GeneralInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GeneralInfo getDataReader_com(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                GeneralInfo info = new GeneralInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GeneralInfo getDataReader_search(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                GeneralInfo info = new GeneralInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private GeneralInfo getDataReader_ext(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                GeneralInfo info = new GeneralInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Deleteitem(iSqlTransaction trans, string iid)
        {
            try
            {
                if (trans == null || string.IsNullOrEmpty(iid)) return false;

                string SQL = "UPDATE " + TABLENAME + " SET status=" + (int)CConstants.State.Status.Deleted + " WHERE id IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleterecursive(iSqlTransaction trans, string iid)
        {
            try
            {
                if (trans == null || string.IsNullOrEmpty(iid)) return false;

                string SQL_SELECT = "SELECT id FROM " + TABLENAME + " WHERE pid IN (" + iid + ")";
                string strIid = "";
                using (iSqlDataReader dar = HELPER.executeReader(trans, SQL_SELECT))
                {
                    while (dar.Read())
                    {
                        int id = dar.IsDBNull(0) ? -1 : dar.GetInt32(0);
                        strIid += id + ",";
                    }
                    if (!string.IsNullOrEmpty(strIid))
                        strIid = strIid.Remove(strIid.LastIndexOf(","), 1);
                }
                this.Deleterecursive(trans, strIid);

                string SQL_DELETE = "UPDATE " + TABLENAME + " SET status=" + (int)CConstants.State.Status.Deleted + " WHERE id IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL_DELETE);

                SQL_DELETE = "UPDATE " + TABLENAME + " SET status=" + (int)CConstants.State.Status.Deleted + " WHERE pid IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL_DELETE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region public methods
        public static string Get_Searchquery(string keywords)
        {
            try
            {
                if (string.IsNullOrEmpty(keywords)) return "";

               // string[] arr_keyword = CFunctions.analys_keyword(keywords);
                string[] arr_keyword = new string[1];
                arr_keyword[0] = keywords;
                string template = "(A.forsearch LIKE '%$VAR_KEYWORD$%')";
                string vlreturn = "";
                SqlHelper helper = new SqlHelper();
                for (int i = 0; i < arr_keyword.Length; i++)
                {
                    string keyword = helper.sqlEscape(CFunctions.to_nonunicode(arr_keyword[i]));
                    if (keyword == "") continue;
                    string item = template.Replace("$VAR_KEYWORD$", keyword);
                    if (i < arr_keyword.Length - 1)
                        vlreturn += item + " OR ";
                    else
                        vlreturn += item;
                }
                return vlreturn;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region IGeneral Members
        public bool Delete(string iid, bool recursive)
        {
            try
            {
                if (string.IsNullOrEmpty(iid)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (recursive)
                                this.Deleterecursive(trans, iid);
                            else
                                this.Deleteitem(trans, iid);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GeneralInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                GeneralInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Getlist(int cid, int pid, string sortexp, string sortdir)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string Setof_Catalogue = cid == 0 ? "" : (new CCategory(LANG)).Get_setof(cid, "");
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(sortexp, sortdir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.pid=" + pid;
                    if (pid == 0)
                        SQL += cid == 0 ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 ";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Search(int belongto, string belongtoname, string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING_SEARCH.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) || categoryid=="0" ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + belongto + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_search(dar);
                            info.Belongto = belongto;
                            info.Belongtoname = belongtoname;
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Searchcount(iConn, belongto, categoryid, Searchquery);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, int belongto, string categoryid, string Searchquery)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT_SEARCH.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += CFunctions.IsNullOrEmpty(categoryid) || categoryid == "0" ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + belongto + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public GeneralInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                GeneralInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public GeneralInfo Wcmm_Getinfo_com(int id)
        {
            if (id == -1) return null;
            try
            {
                GeneralInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO_COM + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader_com(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Wcmm_Getlist(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return null;
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO_COM;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Wcmm_Getlist(int cid, int pid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.pid=" + pid;
                    if (pid == 0)
                        SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Wcmm_Getlist_buildmenu(int cid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Wcmm_Getlist_com(int status, ListOptions options, out int numResults)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += status == (int)CConstants.State.Status.None ? "" : " AND A.status=" + status;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount_com(iConn, status);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount_com(iSqlConnection iConn, int status)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += status == (int)CConstants.State.Status.None ? "" : " AND status=" + status;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<GeneralInfo> Wcmm_Search_com(string keywords, int status, ListOptions options, out int numResults)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.status=" + status;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount_com(iConn, keywords, status);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount_com(iSqlConnection iConn, string keywords, int status)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT_COM.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND status=" + status;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<GeneralInfo> Waitting_Getlist(int status, int belongto, string belongtoname)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO_COM;
                    SQL += " AND A.status=" + status;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            info.Belongto = belongto;
                            info.Belongtoname = belongtoname;
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Waitting_Search(string keywords, int status, int belongto, string belongtoname)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO_COM;
                    SQL += " AND A.status=" + status;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            info.Belongto = belongto;
                            info.Belongtoname = belongtoname;
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Waitting_Count(int status)
        {
            try
            {
                int numResults = 0;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                    SQL += status == (int)CConstants.State.Status.None ? "" : " AND status=" + status;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                        {
                            numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                        }
                    }
                    iConn.Close();
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<GeneralInfo> Wcmm_Getlist_cid(int cid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING_COM.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatenum(string id, string column, object value, int optional)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    switch (optional)
                    {
                        case CConstants.NUM_DIRECTLY:
                            valueset = value.ToString();
                            break;
                        case CConstants.NUM_INCREASE:
                            valueset = column + "+1";
                            break;
                        case CConstants.NUM_INCREASEADD:
                            valueset = column + "+" + value.ToString();
                            break;
                        case CConstants.NUM_DECREASE:
                            valueset = column + "-1";
                            break;
                        case CConstants.NUM_DECREASESUB:
                            valueset = column + "-" + value.ToString();
                            break;
                    }
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatepis(string id, int isAdd, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL1 = "SELECT pid FROM " + TABLENAME + " WHERE id<>0 AND id IN(" + id + ")";
                    string valueset = value.ToString();
                    if (isAdd == CConstants.NUM_INCREASE)
                        valueset = "pis+" + valueset;
                    else if (isAdd == CConstants.NUM_DECREASE)
                        valueset = "pis-" + valueset;
                    string SQL2 = "UPDATE " + TABLENAME + " SET pis=" + valueset + " WHERE id IN(" + SQL1 + ")";
                    HELPER.executeNonQuery(iConn, SQL2);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GeneralInfo> Wcmm_Getlist_extend(int belongto, int aid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT A.name, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A." + options.SortExp + (options.SortDir == "Ascending" ? " ASC" : " DESC") + ") AS rownumber FROM " + TABLENAME + " AS A"
                        + " INNER JOIN " + LANG + CConstants.TBDBPREFIX + "extenditem AS J ON J.iid=A.id";
                    SQL += belongto == 0 ? "" : " AND J.belongto=" + belongto;
                    SQL += aid == 0 ? "" : " AND J.aid=" + aid;
                    SQL += " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Wcmm_Search_extend(string keywords, int belongto, int aid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT A.name, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A." + options.SortExp + (options.SortDir == "Ascending" ? " ASC" : " DESC") + ") AS rownumber FROM " + TABLENAME + " AS A"
                        + " INNER JOIN " + LANG + CConstants.TBDBPREFIX + "extenditem AS J ON J.iid=A.id";
                    SQL += belongto == 0 ? "" : " AND J.belongto=" + belongto;
                    SQL += aid == 0 ? "" : " AND J.aid=" + aid;
                    SQL += " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);

                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_com(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GeneralInfo> Getlist_extend(int belongto, int aid, ListOptions options)
        {
            try
            {
                List<GeneralInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT A.name, A.introduce, A.description, A.filepreview, A.cid, A.iconex, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A." + options.SortExp + (options.SortDir == "Ascending" ? " ASC" : " DESC") + ") AS rownumber FROM " + TABLENAME + " AS A"
                        + " INNER JOIN " + LANG + CConstants.TBDBPREFIX + "extenditem AS J ON J.iid=A.id";
                    SQL += belongto == 0 ? "" : " AND J.belongto=" + belongto;
                    SQL += aid == 0 ? "" : " AND J.aid=" + aid;
                    SQL += " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            GeneralInfo info = this.getDataReader_ext(dar);
                            if (arr == null)
                                arr = new List<GeneralInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CLibraries
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "libraries";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, introduce, description, filepreview, cid, orderd, status, markas, iconex, timecreate, timeupdate, username, allowcomment, album, viewcounter, relateditem, forsearch, id)" +
            " VALUES(@CODE, @NAME, @INTRODUCE, @DESCRIPTION, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMECREATE, @TIMEUPDATE, @USERNAME, @ALLOWCOMMENT, @ALBUM, @VIEWCOUNTER, @RELATEDITEM, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, introduce=@INTRODUCE, description=@DESCRIPTION, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timecreate=@TIMECREATE, timeupdate=@TIMEUPDATE, username=@USERNAME, allowcomment=@ALLOWCOMMENT, album=@ALBUM, viewcounter=@VIEWCOUNTER, relateditem=@RELATEDITEM, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.introduce, A.description, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.album, A.viewcounter, A.relateditem, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.introduce, A.description, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.album, A.viewcounter, A.relateditem, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CLibraries(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_INTRODUCE = "@INTRODUCE";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMECREATE = "@TIMECREATE";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_ALLOWCOMMENT = "@ALLOWCOMMENT";
        private string PARM_ALBUM = "@ALBUM";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_RELATEDITEM = "@RELATEDITEM";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_INTRODUCE, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),							
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMECREATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ALLOWCOMMENT, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ALBUM, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_RELATEDITEM, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, LibrariesInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Introduce);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecreate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Allowcomment;
                parms[++i].Value = info.Album;
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = CFunctions.SetDBString(info.Relateditem);
                parms[++i].Value = CFunctions.install_keyword(info.Code) + " " + CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Introduce) + " " + CFunctions.install_keyword(info.Description) + " " + CFunctions.install_keyword(info.Cname);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private LibrariesInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                LibrariesInfo info = new LibrariesInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timecreate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Allowcomment = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Album = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Relateditem = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, LibrariesInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ILibraries Members
        public bool Save(LibrariesInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public LibrariesInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                LibrariesInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LibrariesInfo> Getlist(int cid, ListOptions options, out int numResults)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string Setof_Catalogue = cid == 0 ? "" : (new CCategory(LANG)).Get_setof(cid, "");
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, Setof_Catalogue, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, string Setof_Catalogue, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(Setof_Catalogue) ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<LibrariesInfo> Search(int cid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string Setof_Catalogue = cid == 0 ? "" : (new CCategory(LANG)).Get_setof(cid, "");
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, Setof_Catalogue, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string Setof_Catalogue, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(Setof_Catalogue) ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<LibrariesInfo> Getlistrelated(string relateditem, ListOptions options)
        {
            if (CFunctions.IsNullOrEmpty(relateditem)) return null;
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id IN(" + relateditem + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LibrariesInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                LibrariesInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LibrariesInfo> Wcmm_Getlist(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return null;
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LibrariesInfo> Wcmm_Getlist(int cid)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<LibrariesInfo> Wcmm_Getlist(int cid, ListOptions options, out int numResults)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string Setof_Catalogue = cid == 0 ? "" : (new CCategory(LANG)).Get_setof(cid, "");
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, Setof_Catalogue, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, string Setof_Catalogue, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(Setof_Catalogue) ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<LibrariesInfo> Wcmm_Search(int cid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Setof_Catalogue = cid == 0 ? "" : (new CCategory(LANG)).Get_setof(cid, "");
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, Setof_Catalogue, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string Setof_Catalogue, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(Setof_Catalogue) ? "" : " AND A.cid IN(" + Setof_Catalogue + ")";
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public List<LibrariesInfo> Wcmm_Report(SearchInfo isearch, ListOptions options, out int numResults)
        {
            try
            {
                List<LibrariesInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    isearch.Setof_Category = isearch.Cid == 0 ? "" : (new CCategory(LANG)).Get_setof(isearch.Cid, "");
                    SQL += isearch.Cid == 0 ? "" : " AND A.cid IN(" + isearch.Setof_Category + ")";
                    isearch.Searchquery = CGeneral.Get_Searchquery(isearch.Keywords);
                    SQL += CFunctions.IsNullOrEmpty(isearch.Searchquery) ? "" : " AND (" + isearch.Searchquery + ")";
                    SQL += " AND (A.timeupdate BETWEEN '" + isearch.Datefr + "' AND '" + isearch.Dateto + "')";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            LibrariesInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<LibrariesInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Reportcount(iConn, isearch, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Reportcount(iSqlConnection iConn, SearchInfo isearch, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.IsNullOrEmpty(isearch.Setof_Category) ? "" : " AND A.cid IN(" + isearch.Setof_Category + ")";
                SQL += string.IsNullOrEmpty(isearch.Searchquery) ? "" : " AND (" + isearch.Searchquery + ")";
                SQL += " AND (A.timeupdate BETWEEN '" + isearch.Datefr + "' AND '" + isearch.Dateto + "')";
                    
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMenu
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "menu";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, note, navigateurl, tooltip, attributes, applyattributeschild, visible, cid, orderd, status, markas, iconex, timeupdate, username, pis, pid, depth, cataloguetypeofid, catalogueid, insertcatalogue, noroot, forsearch, id)" +
            " VALUES(@CODE, @NAME, @NOTE, @NAVIGATEURL, @TOOLTIP, @ATTRIBUTES, @APPLYATTRIBUTESCHILD, @VISIBLE, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMEUPDATE, @USERNAME, @PIS, @PID, @DEPTH, @CATALOGUETYPEOFID, @CATALOGUEID, @INSERTCATALOGUE, @NOROOT, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, note=@NOTE, navigateurl=@NAVIGATEURL, tooltip=@TOOLTIP, attributes=@ATTRIBUTES, applyattributeschild=@APPLYATTRIBUTESCHILD, visible=@VISIBLE, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timeupdate=@TIMEUPDATE, username=@USERNAME, pis=@PIS, pid=@PID, depth=@DEPTH, cataloguetypeofid=@CATALOGUETYPEOFID, catalogueid=@CATALOGUEID, insertcatalogue=@INSERTCATALOGUE, noroot=@NOROOT, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.note, A.navigateurl, A.tooltip, A.attributes, A.applyattributeschild, A.visible, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.cataloguetypeofid, A.catalogueid, A.insertcatalogue, A.noroot, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.note, A.navigateurl, A.tooltip, A.attributes, A.applyattributeschild, A.visible, A.cid, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.pis, A.pid, A.depth, A.cataloguetypeofid, A.catalogueid, A.insertcatalogue, A.noroot, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CMenu()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CMenu(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_NOTE = "@NOTE";
        private string PARM_NAVIGATEURL = "@NAVIGATEURL";
        private string PARM_TOOLTIP = "@TOOLTIP";
        private string PARM_ATTRIBUTES = "@ATTRIBUTES";
        private string PARM_APPLYATTRIBUTESCHILD = "@APPLYATTRIBUTESCHILD";
        private string PARM_VISIBLE = "@VISIBLE";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PIS = "@PIS";
        private string PARM_PID = "@PID";
        private string PARM_DEPTH = "@DEPTH";
        private string PARM_CATALOGUETYPEOFID = "@CATALOGUETYPEOFID";
        private string PARM_CATALOGUEID = "@CATALOGUEID";
        private string PARM_INSERTCATALOGUE = "@INSERTCATALOGUE";
        private string PARM_NOROOT = "@NOROOT";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAVIGATEURL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TOOLTIP, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ATTRIBUTES, iSqlType.Field_tString),
					    new iSqlParameter(PARM_APPLYATTRIBUTESCHILD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_VISIBLE, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PIS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_DEPTH, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CATALOGUETYPEOFID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CATALOGUEID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_INSERTCATALOGUE, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_NOROOT, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MenuInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = CFunctions.SetDBString(info.Navigateurl);
                parms[++i].Value = CFunctions.SetDBString(info.Tooltip);
                parms[++i].Value = CFunctions.SetDBString(info.Attributes);
                parms[++i].Value = info.ApplyAttributesChild;
                parms[++i].Value = info.Visible;
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Pis;
                parms[++i].Value = info.Pid;
                parms[++i].Value = info.Depth;
                parms[++i].Value = info.Cataloguetypeofid;
                parms[++i].Value = info.Catalogueid;
                parms[++i].Value = info.Insertcatalogue;
                parms[++i].Value = info.Noroot;
                parms[++i].Value = CFunctions.install_keyword(info.Name);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MenuInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MenuInfo info = new MenuInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Navigateurl = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Tooltip = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Attributes = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.ApplyAttributesChild = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Visible = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Pis = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Pid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Depth = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Cataloguetypeofid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Catalogueid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Insertcatalogue = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Noroot = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MenuInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region IMenu Members
        public bool Save(MenuInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MenuInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                MenuInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Get_setof(int pid, string listin)
        {
            try
            {
                string list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT id, pis FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            int iid = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                            int pis = dar.IsDBNull(1) ? 0 : dar.GetInt32(1);
                            if (pis != 0)
                                list = this.Get_setof(iid, list);

                            list += iid + ",";
                        }
                        //listin += CFunctions.IsNullOrEmpty(list) ? pid.ToString() : list.Remove(list.Length - 1);
                        listin += list + pid.ToString();
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuInfo> Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<MenuInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenuInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Getlistcount(iConn, cid, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, int cid, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public MenuInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MenuInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<MenuInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenuInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuInfo> Wcmm_Getlist(int cid, int pid)
        {
            try
            {
                List<MenuInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenuInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuInfo> Wcmm_Getlist(int cid, int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<MenuInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenuInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, cid, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int cid, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                SQL += " AND A.pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<MenuInfo> Wcmm_Search(int cid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<MenuInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenuInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, cid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int cid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += cid == 0 ? "" : " AND A.cid=" + cid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<MenuInfo> Wcmm_Getlist_parent(int pid)
        {
            try
            {
                List<MenuInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Wcmm_Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<MenuInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenuInfo> Wcmm_Getlist_sub(int pid, List<MenuInfo> listin)
        {
            try
            {
                List<MenuInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenuInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Wcmm_Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<MenuInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<MenuInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Get_SetofCatalogue(int cid)
        {
            try
            {
                string vlreturn = "";
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT DISTINCT catalogueid FROM " + TABLENAME + " WHERE catalogueid<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += " AND cid=" + cid;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            int catalogueid = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                            vlreturn += catalogueid + ",";
                        }
                    }
                    iConn.Close();
                }
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMenutypeof
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "menutypeof";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, path, note, insertbreak, status, timeupdate, username, forsearch, id)" +
            " VALUES(@CODE, @NAME, @PATH, @NOTE, @INSERTBREAK, @STATUS, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, path=@PATH, note=@NOTE, insertbreak=@INSERTBREAK, status=@STATUS, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.path, A.note, A.insertbreak, A.status, A.timeupdate, A.username, A.id FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CMenutypeof()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CMenutypeof(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_PATH = "@PATH";
        private string PARM_NOTE = "@NOTE";
        private string PARM_INSERTBREAK = "@INSERTBREAK";
        private string PARM_STATUS = "@STATUS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PATH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_INSERTBREAK, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MenutypeofInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Path);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = info.Insertbreak;
                parms[++i].Value = info.Status;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.install_keyword(info.Name);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MenutypeofInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MenutypeofInfo info = new MenutypeofInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Path = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Insertbreak = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MenutypeofInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IMenutypeof Members
        public bool Save(MenutypeofInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MenutypeofInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MenutypeofInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MenutypeofInfo Wcmm_Getinfo(string code)
        {
            if (CFunctions.IsNullOrEmpty(code)) return null;
            try
            {
                MenutypeofInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.code=@CODE";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = code;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenutypeofInfo> Wcmm_Getlist(int status)
        {
            try
            {
                List<MenutypeofInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status=" + status;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenutypeofInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenutypeofInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MenutypeofInfo> Wcmm_Search(int status, string keywords)
        {
            try
            {
                List<MenutypeofInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status=" + status;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MenutypeofInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MenutypeofInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMember
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "member";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(username, password, firstname, lastname, email, pin, status, markas, timeupdate, logincache, loginfirst, autosave, temporarycode, grouptype, filepreview, ranking, vote, ratingweight, forsearch, id)" +
            " VALUES(@USERNAME, @PASSWORD, @FIRSTNAME, @LASTNAME, @EMAIL, @PIN, @STATUS, @MARKAS, @TIMEUPDATE, @LOGINCACHE, @LOGINFIRST, @AUTOSAVE, @TEMPORARYCODE, @GROUPTYPE, @FILEPREVIEW, @RANKING, @VOTE, @RATINGWEIGHT, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET username=@USERNAME, password=@PASSWORD, firstname=@FIRSTNAME, lastname=@LASTNAME, email=@EMAIL, pin=@PIN, status=@STATUS, markas=@MARKAS, timeupdate=@TIMEUPDATE, logincache=@LOGINCACHE, loginfirst=@LOGINFIRST, autosave=@AUTOSAVE, temporarycode=@TEMPORARYCODE, grouptype=@GROUPTYPE, filepreview=@FILEPREVIEW, ranking=@RANKING, vote=@VOTE, ratingweight=@RATINGWEIGHT, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.username, A.password, A.firstname, A.lastname, A.email, A.pin, A.status, A.markas, A.timeupdate, A.logincache, A.loginfirst, A.autosave, A.temporarycode, A.grouptype, A.filepreview, A.ranking, A.vote, A.ratingweight, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_GETIFOPAGING = "SELECT A.username, A.password, A.firstname, A.lastname, A.email, A.pin, A.status, A.markas, A.timeupdate, A.logincache, A.loginfirst, A.autosave, A.temporarycode, A.grouptype, A.filepreview, A.ranking, A.vote, A.ratingweight, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_CHANGE_PWD = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET password=@PASSWORD WHERE id=@ID";

        public CMember()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = "";
            
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_CHANGE_PWD = SQL_CHANGE_PWD.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CMember(string lang)
        {
            HELPER = new SqlHelper();
            LANG = lang;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = "";
            
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_CHANGE_PWD = SQL_CHANGE_PWD.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PASSWORD = "@PASSWORD";
        private string PARM_FIRSTNAME = "@FIRSTNAME";
        private string PARM_LASTNAME = "@LASTNAME";
        private string PARM_EMAIL = "@EMAIL";
        private string PARM_PIN = "@PIN";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_LOGINCACHE = "@LOGINCACHE";
        private string PARM_LOGINFIRST = "@LOGINFIRST";
        private string PARM_AUTOSAVE = "@AUTOSAVE";
        private string PARM_TEMPORARYCODE = "@TEMPORARYCODE";
        private string PARM_GROUPTYPE = "@GROUPTYPE";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_RANKING = "@RANKING";
        private string PARM_VOTE = "@VOTE";
        private string PARM_RATINGWEIGHT = "@RATINGWEIGHT";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FIRSTNAME, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_LASTNAME, iSqlType.Field_tString),
						new iSqlParameter(PARM_EMAIL, iSqlType.Field_tString),
                        new iSqlParameter(PARM_PIN, iSqlType.Field_tString),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_LOGINCACHE, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_LOGINFIRST, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_AUTOSAVE, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TEMPORARYCODE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_GROUPTYPE, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
                        new iSqlParameter(PARM_RANKING, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_VOTE, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_RATINGWEIGHT, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MemberInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.SetDBString(info.Password);
                parms[++i].Value = CFunctions.SetDBString(info.Firstname);
                parms[++i].Value = CFunctions.SetDBString(info.Lastname);
                parms[++i].Value = CFunctions.SetDBString(info.Email);
                parms[++i].Value = CFunctions.SetDBString(info.PIN);
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = info.Logincache;
                parms[++i].Value = info.Loginfirst;
                parms[++i].Value = info.Autosave;
                parms[++i].Value = CFunctions.SetDBString(info.Temporarycode);
                parms[++i].Value = info.Grouptype;
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Ranking;
                parms[++i].Value = info.Vote;
                parms[++i].Value = info.Ratingweight;
                parms[++i].Value = CFunctions.install_keyword(info.Fullname) + " " + CFunctions.install_keyword(info.Username);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MemberInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MemberInfo info = new MemberInfo();
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Password = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Firstname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Lastname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.PIN = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Logincache = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Loginfirst = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Autosave = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Temporarycode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Grouptype = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Ranking = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Vote = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Ratingweight = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                info.iProfile = new CMeProfile(LANG).Getinfo(info.Id);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MemberInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IMember Members
        public bool Save(MemberInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                            {
                                if (info.iProfile != null)
                                {
                                    info.iProfile.Id = info.Id;
                                    new CMeProfile(LANG).Save(info.iProfile);
                                }
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MemberInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MemberInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MemberInfo Getinfo(string username)
        {
            if (CFunctions.IsNullOrEmpty(username)) return null;
            try
            {
                MemberInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.username=@USERNAME";
                    //SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    //SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = username;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public CConstants.State.Existed Exist(MemberInfo info)
        {
            if (info == null) return CConstants.State.Existed.None;
            try
            {
                CConstants.State.Existed vlreturn = CConstants.State.Existed.None;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    if (HELPER.isExist(iConn, TABLENAME, "username", info.Username, info.Id))
                    {
                        vlreturn = CConstants.State.Existed.Name;
                        goto closeConn;
                    }

                closeConn: iConn.Close();
                }
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MemberInfo Login(MemberInfo info)
        {
            try
            {
                MemberInfo _info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND (A.username=@USERNAME) AND (A.password=@PASSWORD)";

                    iSqlParameter[] parms = new iSqlParameter[]{
													   new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
													   new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString)
					};
                    parms[0].Value = info.Username;
                    parms[1].Value = info.Password;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            _info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return _info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ChangePwd(MemberInfo info)
        {
            try
            {
                if (info == null) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    iSqlParameter[] parms = new iSqlParameter[]{
						new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString),
						new iSqlParameter(PARM_ID, iSqlType.Field_tInterger)
					};
                    parms[0].Value = info.Password;
                    parms[1].Value = info.Id;
                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL_CHANGE_PWD, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MemberInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                MemberInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MemberInfo Wcmm_Getinfo(string username)
        {
            if (CFunctions.IsNullOrEmpty(username)) return null;
            try
            {
                MemberInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.username=@USERNAME";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = username;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MemberInfo> Wcmm_Getlist(int grouptype, ListOptions options, out int numResults)
        {
            try
            {
                List<MemberInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += grouptype == -1 ? "" : " AND A.grouptype=" + grouptype;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MemberInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MemberInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, grouptype, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int grouptype, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += grouptype == -1 ? "" : " AND A.grouptype=" + grouptype;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<MemberInfo> Wcmm_Search(int grouptype, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<MemberInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += grouptype == -1 ? "" : " AND A.grouptype=" + grouptype;
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MemberInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MemberInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, grouptype, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int grouptype, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += grouptype == -1 ? "" : " AND A.grouptype=" + grouptype;
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<MemberInfo> Wcmm_Getlist(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return null;
            try
            {
                List<MemberInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            MemberInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<MemberInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Wcmm_Getlistemail(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return "";
            try
            {
                string listemail = "";
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "SELECT username FROM " + TABLENAME + " WHERE id<>0 AND status<>" + (int)CConstants.State.Status.Deleted;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            string username = dar.IsDBNull(0) ? string.Empty : dar.GetString(0);
                            listemail += username + ";";
                        }
                    }
                    iConn.Close();
                }
                return listemail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updaterating(int memberid, int ratingweight)
        {
            try
            {
                if (memberid == 0) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    int Vote = 0;
                    int Ratingweight = 0;
                    string SQL = "SELECT A.vote, A.ratingweight FROM " + TABLENAME + " AS A WHERE A.id=" + memberid;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        if (dar.Read())
                        {
                            int i = -1;
                            Vote = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                            Ratingweight = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                        }
                        dar.Close();
                    }

                    Ratingweight = Vote == 0 ? ratingweight : Convert.ToInt32((Ratingweight + ratingweight) / 2);
                    Vote = Vote + 1;
                    string Ranking = (Vote % 2 == 0 ? (Ratingweight > 3 ? ", ranking = ranking + 1" : ", ranking = ranking - 1") : "");
                    SQL = "UPDATE " + TABLENAME + " SET vote=" + Vote + ", ratingweight=" + Ratingweight + Ranking + " WHERE id=" + memberid;
                    HELPER.executeNonQuery(iConn, SQL);

                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CMeProfile
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "meprofile";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(address, zipcode, state, nationalid, cityid, districtid, phone, about, blog, homepage, facebook, twitter, youtube, flickr, skype, yahoo, birthday, profession, id)" +
            " VALUES(@ADDRESS, @ZIPCODE, @STATE, @NATIONALID, @CITYID, @DISTRICTID, @PHONE, @ABOUT, @BLOG, @HOMEPAGE, @FACEBOOK, @TWITTER, @YOUTUBE, @FLICKR, @SKYPE, @YAHOO, @BIRTHDAY, @PROFESSION, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET address=@ADDRESS, zipcode=@ZIPCODE, state=@STATE, nationalid=@NATIONALID, cityid=@CITYID, districtid=@DISTRICTID, phone=@PHONE, about=@ABOUT, blog=@BLOG, homepage=@HOMEPAGE, facebook=@FACEBOOK, twitter=@TWITTER, youtube=@YOUTUBE, flickr=@FLICKR, skype=@SKYPE, yahoo=@YAHOO, birthday=@BIRTHDAY, profession=@PROFESSION WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.address, A.zipcode, A.state, A.nationalid, C1.name AS nationalname, A.cityid, C2.name AS cityname, A.districtid, C3.name AS districtname, A.phone, A.about, A.blog, A.homepage, A.facebook, A.twitter, A.youtube, A.flickr, A.skype, A.yahoo, A.birthday, A.profession, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0";
        private string SQL_GETIFOPAGING = "SELECT A.address, A.zipcode, A.state, A.nationalid, C1.name AS nationalname, A.cityid, C2.name AS cityname, A.districtid, C3.name AS districtname, A.phone, A.about, A.blog, A.homepage, A.facebook, A.twitter, A.youtube, A.flickr, A.skype, A.yahoo, A.birthday, A.profession, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0";

        public CMeProfile()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C1 ON C1.id=A.nationalid"
                + " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C2 ON C2.id=A.cityid"
                + " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C3 ON C3.id=A.districtid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }
        public CMeProfile(string lang)
        {
            HELPER = new SqlHelper();
            LANG = lang;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C1 ON C1.id=A.nationalid"
                + " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C2 ON C2.id=A.cityid"
                + " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C3 ON C3.id=A.districtid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_ADDRESS = "@ADDRESS";
        private string PARM_ZIPCODE = "@ZIPCODE";
        private string PARM_STATE = "@STATE";
        private string PARM_NATIONALID = "@NATIONALID";
        private string PARM_CITYID = "@CITYID";
        private string PARM_DISTRICTID = "@DISTRICTID";
        private string PARM_PHONE = "@PHONE";
        private string PARM_ABOUT = "@ABOUT";
        private string PARM_BLOG = "@BLOG";
        private string PARM_HOMEPAGE = "@HOMEPAGE";
        private string PARM_FACEBOOK = "@FACEBOOK";
        private string PARM_TWITTER = "@TWITTER";
        private string PARM_YOUTUBE = "@YOUTUBE";
        private string PARM_FLICKR = "@FLICKR";
        private string PARM_SKYPE = "@SKYPE";
        private string PARM_YAHOO = "@YAHOO";
        private string PARM_BIRTHDAY = "@BIRTHDAY";
        private string PARM_PROFESSION = "@PROFESSION";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ADDRESS, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ZIPCODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_STATE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NATIONALID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_CITYID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_DISTRICTID, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PHONE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ABOUT, iSqlType.Field_tString),
                        new iSqlParameter(PARM_BLOG, iSqlType.Field_tString),
                        new iSqlParameter(PARM_HOMEPAGE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FACEBOOK, iSqlType.Field_tString),
                        new iSqlParameter(PARM_TWITTER, iSqlType.Field_tString),
                        new iSqlParameter(PARM_YOUTUBE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FLICKR, iSqlType.Field_tString),
                        new iSqlParameter(PARM_SKYPE, iSqlType.Field_tString),
                        new iSqlParameter(PARM_YAHOO, iSqlType.Field_tString),
                        new iSqlParameter(PARM_BIRTHDAY, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_PROFESSION, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, MeProfileInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Address);
                parms[++i].Value = CFunctions.SetDBString(info.Zipcode);
                parms[++i].Value = CFunctions.SetDBString(info.State);
                parms[++i].Value = info.Nationalid;
                parms[++i].Value = info.Cityid;
                parms[++i].Value = info.Districtid;
                parms[++i].Value = CFunctions.SetDBString(info.Phone);
                parms[++i].Value = CFunctions.SetDBString(info.About);
                parms[++i].Value = CFunctions.SetDBString(info.Blog);
                parms[++i].Value = CFunctions.SetDBString(info.Homepage);
                parms[++i].Value = CFunctions.SetDBString(info.Facebook);
                parms[++i].Value = CFunctions.SetDBString(info.Twitter);
                parms[++i].Value = CFunctions.SetDBString(info.Youtube);
                parms[++i].Value = CFunctions.SetDBString(info.Flickr);
                parms[++i].Value = CFunctions.SetDBString(info.Skype);
                parms[++i].Value = CFunctions.SetDBString(info.Yahoo);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Birthday);
                parms[++i].Value = CFunctions.SetDBString(info.Profession);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MeProfileInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                MeProfileInfo info = new MeProfileInfo();
                info.Address = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Zipcode = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.State = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Nationalid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Nationalname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cityid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cityname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Districtid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Districtname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Phone = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.About = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Blog = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Homepage = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Facebook = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Twitter = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Youtube = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Flickr = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Skype = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Yahoo = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Birthday = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Profession = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, MeProfileInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    //info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IMeProfile Members
        public bool Save(MeProfileInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MeProfileInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                MeProfileInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CNews
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "news";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, introduce, description, tag, filepreview, cid, orderd, status, markas, iconex, timecreate, timeupdate, username, allowcomment, author, timeexpire, viewcounter, url, relateditem, forsearch, id)" +
            " VALUES(@CODE, @NAME, @INTRODUCE, @DESCRIPTION, @TAG, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMECREATE, @TIMEUPDATE, @USERNAME, @ALLOWCOMMENT, @AUTHOR, @TIMEEXPIRE, @VIEWCOUNTER, @URL, @RELATEDITEM, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, introduce=@INTRODUCE, description=@DESCRIPTION, tag=@TAG, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timecreate=@TIMECREATE, timeupdate=@TIMEUPDATE, username=@USERNAME, allowcomment=@ALLOWCOMMENT, author=@AUTHOR, timeexpire=@TIMEEXPIRE, viewcounter=@VIEWCOUNTER, url=@URL, relateditem=@RELATEDITEM, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.introduce, A.description, A.tag, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.author, A.timeexpire, A.viewcounter, A.url, A.relateditem, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.introduce, A.description, A.tag, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.author, A.timeexpire, A.viewcounter, A.url, A.relateditem, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CNews(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_INTRODUCE = "@INTRODUCE";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_TAG = "@TAG";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMECREATE = "@TIMECREATE";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_ALLOWCOMMENT = "@ALLOWCOMMENT";
        private string PARM_AUTHOR = "@AUTHOR";
        private string PARM_TIMEEXPIRE = "@TIMEEXPIRE";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_URL = "@URL";
        private string PARM_RELATEDITEM = "@RELATEDITEM";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_INTRODUCE, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),
					    new iSqlParameter(PARM_TAG, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMECREATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ALLOWCOMMENT, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_AUTHOR, iSqlType.Field_tString),
                        new iSqlParameter(PARM_TIMEEXPIRE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_URL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_RELATEDITEM, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, NewsInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Introduce);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = CFunctions.SetDBString(info.Tag);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecreate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Allowcomment;
                parms[++i].Value = CFunctions.SetDBString(info.Author);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeexpire);
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = CFunctions.SetDBString(info.Url);
                parms[++i].Value = CFunctions.SetDBString(info.Relateditem);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Introduce) + " " + CFunctions.install_keyword(info.Description) + " " + CFunctions.install_keyword(info.Tag) + " " + CFunctions.install_keyword(info.Author) + " " + CFunctions.install_keyword(info.Url);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private NewsInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                NewsInfo info = new NewsInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Tag = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timecreate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Allowcomment = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Author = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeexpire = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Url = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Relateditem = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                info.lCategory = (new CCategory(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.News);
                info.lCategoryattr = (new CCategoryattr(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.News);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, NewsInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region INews Members
        public bool Save(NewsInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                            {
                                new CItemcategory(LANG).Save(info.Id, Webcmm.Id.News, info.Categoryid);
                                new CItemcategoryattr(LANG).Save(info.Id, Webcmm.Id.News, info.Categoryattrid);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }

                return true; ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateAliasInfo(NewsInfo info)
        {
            bool kq = false;
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE vndd_news SET alias = '";
                    SQL += CFunctions.install_urlname(info.Name).Replace(".aspx", "")+"'";
                    SQL += " Where id = " + info.Id;
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                kq = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return kq;
        }
        public NewsInfo Getinfo(string alias)
        {
            if (alias == "") return null;
            try
            {
                NewsInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.alias='" + alias + "'";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = 1;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public NewsInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                NewsInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsInfo> Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetDictCount(iSqlConnection iConn, string categoryid)
        {
            try
            {
                int numResults = 0;
                string SQL =
                    @"select COUNT(*) as sl from vndd_itemcategory a left join 
vndd_news b on a.iid = b.id where b.status <> 3 and a.categoryid = " + categoryid;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        private int Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<NewsInfo> Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<NewsInfo> Getlistrelated(string relateditem)
        {
            if (CFunctions.IsNullOrEmpty(relateditem)) return null;
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id IN(" + relateditem + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsInfo> Getlistattr(string categoryid, string attrid, string attrcode, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL += CFunctions.IsNullOrEmpty(attrid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL += CFunctions.IsNullOrEmpty(attrcode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "categoryattr AS CA ON ICA.categoryid=CA.id WHERE CA.code IN (" + attrcode + ") AND ICA.belongto=" + Webcmm.Id.News + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcountattr(iConn, categoryid, attrid, attrcode, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcountattr(iSqlConnection iConn, string categoryid, string attrid, string attrcode, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += CFunctions.IsNullOrEmpty(attrid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += CFunctions.IsNullOrEmpty(attrcode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "categoryattr AS CA ON ICA.categoryid=CA.id WHERE CA.code IN (" + attrcode + ") AND ICA.belongto=" + Webcmm.Id.News + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<NewsInfo> Getlistcate(string categoryid, string categorycode, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL += CFunctions.IsNullOrEmpty(categorycode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "category AS CA ON ICA.categoryid=CA.id WHERE CA.code IN (" + categorycode + ") AND ICA.belongto=" + Webcmm.Id.News + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcountcate(iConn, categoryid, categorycode, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcountcate(iSqlConnection iConn, string categoryid, string categorycode, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += CFunctions.IsNullOrEmpty(categorycode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "category AS CA ON ICA.categoryid=CA.id WHERE CA.code IN (" + categorycode + ") AND ICA.belongto=" + Webcmm.Id.News + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public List<NewsInfo> Getlistdic(int cidroot, string listby, string listkey, string listlang, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string categoryid, varsqlabc;
                    if (listby == "abc")
                    {
                        options.SortExp = listlang == "en" ? "url" : "name";
                        options.SortDir = "Ascending";
                        categoryid = cidroot.ToString();
                        varsqlabc = CFunctions.IsNullOrEmpty(listkey) ? "" : (" AND A." + (listlang == "en" ? "url" : "name") + " LIKE '" + listkey + "%'");
                    }
                    else if(listby == "cat")
                    {
                        options.SortExp = "orderd";
                        options.SortDir = "Descending";
                        categoryid = CFunctions.IsNullOrEmpty(listkey) ? cidroot.ToString() : listkey;
                        varsqlabc = "";
                    }
                    else 
                    {
                        options.SortExp = "orderd";
                        options.SortDir = "Descending";
                        categoryid = cidroot.ToString();
                        string Searchquery = CGeneral.Get_Searchquery(listkey);
                        varsqlabc = CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    }
            
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL += varsqlabc;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcountdic(iConn, categoryid, varsqlabc, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcountdic(iSqlConnection iConn, string categoryid, string varsqlabc, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += varsqlabc;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }


        public NewsInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                NewsInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsInfo> Wcmm_Getlist(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return null;
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsInfo> Wcmm_Getlist(int cid)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<NewsInfo> Wcmm_Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<NewsInfo> Wcmm_Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<NewsInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            NewsInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<NewsInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.News + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CRSSResource
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "rssresource";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, websiteurl, rssurl, timelastestget, nodecontent, nodetitle, nodeintroduce, cid, orderd, status, markas, iconex, timeupdate, username, forsearch, id)" +
            " VALUES(@CODE, @NAME, @WEBSITEURL, @RSSURL, @TIMELASTESTGET, @NODECONTENT, @NODETITLE, @NODEINTRODUCE, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, websiteurl=@WEBSITEURL, rssurl=@RSSURL, timelastestget=@TIMELASTESTGET, nodecontent=@NODECONTENT, nodetitle=@NODETITLE, nodeintroduce=@NODEINTRODUCE, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.websiteurl, A.rssurl, A.timelastestget, A.nodecontent, A.nodetitle, A.nodeintroduce, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.websiteurl, A.rssurl, A.timelastestget, A.nodecontent, A.nodetitle, A.nodeintroduce, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CRSSResource(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_WEBSITEURL = "@WEBSITEURL";
        private string PARM_RSSURL = "@RSSURL";
        private string PARM_TIMELASTESTGET = "@TIMELASTESTGET";
        private string PARM_NODECONTENT = "@NODECONTENT";
        private string PARM_NODETITLE = "@NODETITLE";
        private string PARM_NODEINTRODUCE = "@NODEINTRODUCE";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_WEBSITEURL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_RSSURL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMELASTESTGET, iSqlType.Field_tDate),
					    new iSqlParameter(PARM_NODECONTENT, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NODETITLE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NODEINTRODUCE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, RSSResourceInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.Code;
                parms[++i].Value = info.Name;
                parms[++i].Value = info.WebsiteUrl;
                parms[++i].Value = info.RSSUrl;
                if (info.Timelastestget.Equals(new DateTime(0)))
                    parms[++i].Value = DBNull.Value;
                else
                    parms[++i].Value = info.Timelastestget;
                parms[++i].Value = info.Nodecontent;
                parms[++i].Value = info.Nodetitle;
                parms[++i].Value = info.Nodeintroduce;
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = info.Iconex;
                if (info.Timeupdate.Equals(new DateTime(0)))
                    parms[++i].Value = DBNull.Value;
                else
                    parms[++i].Value = info.Timeupdate;
                parms[++i].Value = info.Username;
                parms[++i].Value = CFunctions.install_keyword(info.Code) + " " + CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Cname);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private RSSResourceInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                RSSResourceInfo info = new RSSResourceInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.WebsiteUrl = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.RSSUrl = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timelastestget = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Nodecontent = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Nodetitle = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Nodeintroduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, RSSResourceInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IRSSResource Members
        public bool Save(RSSResourceInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RSSResourceInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                RSSResourceInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RSSResourceInfo> Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<RSSResourceInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            RSSResourceInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<RSSResourceInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<RSSResourceInfo> Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<RSSResourceInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            RSSResourceInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<RSSResourceInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public RSSResourceInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                RSSResourceInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RSSResourceInfo Wcmm_Getinfo(string rssurl)
        {
            if (CFunctions.IsNullOrEmpty(rssurl)) return null;
            try
            {
                RSSResourceInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.rssurl=@RSSURL";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_RSSURL, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = rssurl;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RSSResourceInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<RSSResourceInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            RSSResourceInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<RSSResourceInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RSSResourceInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<RSSResourceInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            RSSResourceInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<RSSResourceInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<RSSResourceInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<RSSResourceInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            RSSResourceInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<RSSResourceInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CProduct
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "product";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, introduce, description, features, filepreview, cid, orderd, status, markas, iconex, timecreate, timeupdate, username, viewcounter, provider, advertise, price, pricealter, allowcomment, album, relateditem, relatednews, forsearch, id)" +
            " VALUES(@CODE, @NAME, @INTRODUCE, @DESCRIPTION, @FEATURES, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMECREATE, @TIMEUPDATE, @USERNAME, @VIEWCOUNTER, @PROVIDER, @ADVERTISE, @PRICE, @PRICEALTER, @ALLOWCOMMENT, @ALBUM, @RELATEDITEM, @RELATEDNEWS, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, introduce=@INTRODUCE, description=@DESCRIPTION, features=@FEATURES, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timecreate=@TIMECREATE, timeupdate=@TIMEUPDATE, username=@USERNAME, viewcounter=@VIEWCOUNTER, provider=@PROVIDER, advertise=@ADVERTISE, price=@PRICE, pricealter=@PRICEALTER, allowcomment=@ALLOWCOMMENT, album=@ALBUM, relateditem=@RELATEDITEM, relatednews=@RELATEDNEWS, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.introduce, A.description, A.features, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.viewcounter, A.provider, A.advertise, A.price, A.pricealter, A.allowcomment, A.album, A.relateditem, A.relatednews, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.introduce, A.description, A.features, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.viewcounter, A.provider, A.advertise, A.price, A.pricealter, A.allowcomment, A.album, A.relateditem, A.relatednews, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CProduct(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_INTRODUCE = "@INTRODUCE";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_FEATURES = "@FEATURES";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMECREATE = "@TIMECREATE";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_PROVIDER = "@PROVIDER";
        private string PARM_ADVERTISE = "@ADVERTISE";
        private string PARM_PRICE = "@PRICE";
        private string PARM_PRICEALTER = "@PRICEALTER";
        private string PARM_ALLOWCOMMENT = "@ALLOWCOMMENT";
        private string PARM_ALBUM = "@ALBUM";
        private string PARM_RELATEDITEM = "@RELATEDITEM";
        private string PARM_RELATEDNEWS = "@RELATEDNEWS";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_INTRODUCE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),
					    new iSqlParameter(PARM_FEATURES, iSqlType.Field_tText),
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMECREATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_PROVIDER, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ADVERTISE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PRICE, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_PRICEALTER, iSqlType.Field_tReal),
                        new iSqlParameter(PARM_ALLOWCOMMENT, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ALBUM, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_RELATEDITEM, iSqlType.Field_tString),
                        new iSqlParameter(PARM_RELATEDNEWS, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, ProductInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Introduce);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = CFunctions.SetDBString(info.Features);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = info.Iconex;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecreate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = CFunctions.SetDBString(info.Provider);
                parms[++i].Value = CFunctions.SetDBString(info.Advertise);
                parms[++i].Value = info.Price;
                parms[++i].Value = info.Pricealter;
                parms[++i].Value = info.Allowcomment;
                parms[++i].Value = info.Album;
                parms[++i].Value = CFunctions.SetDBString(info.Relateditem);
                parms[++i].Value = CFunctions.SetDBString(info.Relatednews);
                parms[++i].Value = CFunctions.install_keyword(info.Code) + " " + CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Introduce) + " " + CFunctions.install_keyword(info.Description) + " " + CFunctions.install_keyword(info.Features) + " " + CFunctions.install_keyword(info.Advertise) + " " + CFunctions.install_keyword(info.Provider) + " " + CFunctions.install_keyword(info.Cname);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ProductInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                ProductInfo info = new ProductInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Introduce = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Features = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timecreate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Provider = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Advertise = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Price = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i));
                info.Pricealter = dar.IsDBNull(++i) ? 0 : Math.Round(dar.GetFloat(i), 2);
                info.Allowcomment = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Album = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Relateditem = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Relatednews = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                info.lCategory = (new CCategory(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.Product);
                info.lCategoryattr = (new CCategoryattr(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.Product);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, ProductInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IProduct Members
        public bool Save(ProductInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                            {
                                new CItemcategory(LANG).Save(info.Id, Webcmm.Id.Product, info.Categoryid);
                                new CItemcategoryattr(LANG).Save(info.Id, Webcmm.Id.Product, info.Categoryattrid);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ProductInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                ProductInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductInfo> Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<ProductInfo> Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<ProductInfo> Getlistrelated(string relateditem, ListOptions options)
        {
            if (CFunctions.IsNullOrEmpty(relateditem)) return null;
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id IN(" + relateditem + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductInfo> Getlistattr(string categoryid, int attrid, string attrcode, ListOptions options, out int numResults)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    SQL += attrid == 0 ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    SQL += CFunctions.IsNullOrEmpty(attrcode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "categoryattr AS CA ON ICA.categoryid=CA.id WHERE CA.code='" + attrcode + "' AND ICA.belongto=" + Webcmm.Id.Product + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcountattr(iConn, categoryid, attrid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcountattr(iSqlConnection iConn, string categoryid, int attrid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                SQL += attrid == 0 ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.Product + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public ProductInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                ProductInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductInfo> Wcmm_Getlist(string iid)
        {
            if (iid == "") return null;
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += iid == "" ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductInfo> Wcmm_Getlist(int cid)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductInfo> Wcmm_Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<ProductInfo> Wcmm_Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<ProductInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ProductInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ProductInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Wcmm_Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Product + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CItemcategory
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "itemcategory";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(iid, categoryid, belongto)" +
            " VALUES(@IID, @CATEGORYID, @BELONGTO)";
        private string SQL_GETIFO = "SELECT A.iid, A.categoryid, A.belongto FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.iid<>0 AND A.categoryid<>0";

        public CItemcategory(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_IID = "@IID";
        private string PARM_CATEGORYID = "@CATEGORYID";
        private string PARM_BELONGTO = "@BELONGTO";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_CATEGORYID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, ItemcategoryInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.Iid;
                parms[++i].Value = info.Categoryid;
                parms[++i].Value = info.Belongto;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemcategoryInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                ItemcategoryInfo info = new ItemcategoryInfo();
                info.Iid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Categoryid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Belongto = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, ItemcategoryInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = SQL_INSERT;
                iSqlParameter[] parms = this.getParameter(SQL);
                this.setParameter(parms, info);
                HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, int iid, int belongto)
        {
            try
            {
                if (trans == null) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE iid=" + iid + " AND belongto=" + belongto;
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IItemcategory Members
        public bool Save(int iid, int belongto, string categoryid)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Deleteitem(trans, iid, belongto))
                            {
                                if (!CFunctions.IsNullOrEmpty(categoryid))
                                {
                                    string[] strid = categoryid.Split(',');
                                    if (strid.Length > 0)
                                    {
                                        for (int i = 0; i < strid.Length; i++)
                                        {
                                            ItemcategoryInfo info = new ItemcategoryInfo();
                                            info.Iid = iid;
                                            info.Belongto = belongto;
                                            info.Categoryid = Convert.ToInt32(strid[i]);
                                            this.Saveitem(trans, info);
                                        }
                                    }
                                }
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ItemcategoryInfo> Getlist(int iid, int belongto)
        {
            try
            {
                List<ItemcategoryInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ItemcategoryInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ItemcategoryInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CItemcategoryattr
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "itemcategoryattr";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(iid, categoryid, belongto)" +
            " VALUES(@IID, @CATEGORYID, @BELONGTO)";
        private string SQL_GETIFO = "SELECT A.iid, A.categoryid, A.belongto FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.iid<>0 AND A.categoryid<>0";

        public CItemcategoryattr(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_IID = "@IID";
        private string PARM_CATEGORYID = "@CATEGORYID";
        private string PARM_BELONGTO = "@BELONGTO";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_IID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_CATEGORYID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_BELONGTO, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, ItemcategoryattrInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.Iid;
                parms[++i].Value = info.Categoryid;
                parms[++i].Value = info.Belongto;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private ItemcategoryattrInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                ItemcategoryattrInfo info = new ItemcategoryattrInfo();
                info.Iid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Categoryid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Belongto = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, ItemcategoryattrInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = SQL_INSERT;
                iSqlParameter[] parms = this.getParameter(SQL);
                this.setParameter(parms, info);
                HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, int iid, int belongto)
        {
            try
            {
                if (trans == null) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE iid=" + iid + " AND belongto=" + belongto;
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IItemcategoryattr Members
        public bool Save(int iid, int belongto, string categoryid)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Deleteitem(trans, iid, belongto))
                            {
                                if (!CFunctions.IsNullOrEmpty(categoryid))
                                {
                                    string[] strid = categoryid.Split(',');
                                    if (strid.Length > 0)
                                    {
                                        for (int i = 0; i < strid.Length; i++)
                                        {
                                            ItemcategoryattrInfo info = new ItemcategoryattrInfo();
                                            info.Iid = iid;
                                            info.Belongto = belongto;
                                            info.Categoryid = Convert.ToInt32(strid[i]);
                                            this.Saveitem(trans, info);
                                        }
                                    }
                                }
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ItemcategoryattrInfo> Getlist(int iid, int belongto)
        {
            try
            {
                List<ItemcategoryattrInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            ItemcategoryattrInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<ItemcategoryattrInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CStaticcontent
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "staticcontent";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, code, separatepage, filepath, description, status, timeupdate, username, forsearch, id)" +
            " VALUES(@NAME, @CODE, @SEPARATEPAGE, @FILEPATH, @DESCRIPTION, @STATUS, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, code=@CODE, separatepage=@SEPARATEPAGE, filepath=@FILEPATH, description=@DESCRIPTION, status=@STATUS, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.code, A.separatepage, A.filepath, A.description, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.code, A.separatepage, A.filepath, A.description, A.status, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CStaticcontent()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CStaticcontent(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_CODE = "@CODE";
        private string PARM_SEPARATEPAGE = "@SEPARATEPAGE";
        private string PARM_FILEPATH = "@FILEPATH";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_STATUS = "@STATUS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SEPARATEPAGE, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_FILEPATH, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, StaticcontentInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = info.Separatepage;
                parms[++i].Value = CFunctions.SetDBString(info.Filepath);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = info.Status;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Description);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private StaticcontentInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                StaticcontentInfo info = new StaticcontentInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Separatepage = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Filepath = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? -1 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, StaticcontentInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region IStaticcontent Members
        public bool Save(StaticcontentInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public StaticcontentInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                StaticcontentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public StaticcontentInfo Getinfo(string code)
        {
            if (CFunctions.IsNullOrEmpty(code)) return null;
            try
            {
                StaticcontentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.code=@CODE";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = code;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StaticcontentInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                StaticcontentInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StaticcontentInfo> Wcmm_Getlist()
        {
            try
            {
                List<StaticcontentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            StaticcontentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<StaticcontentInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<StaticcontentInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<StaticcontentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            StaticcontentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<StaticcontentInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<StaticcontentInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<StaticcontentInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            StaticcontentInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<StaticcontentInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CSymbol
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "symbol";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, path, note, sized, status, orderd, timeupdate, username, forsearch, id)" +
            " VALUES(@NAME, @PATH, @NOTE, @SIZED, @STATUS, @ORDERD, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, path=@PATH, note=@NOTE, sized=@SIZED, status=@STATUS, orderd=@ORDERD, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.path, A.note, A.sized, A.status, A.orderd, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.path, A.note, A.sized, A.status, A.orderd, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CSymbol(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_PATH = "@PATH";
        private string PARM_NOTE = "@NOTE";
        private string PARM_SIZED = "@SIZED";
        private string PARM_STATUS = "@STATUS";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PATH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SIZED, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, SymbolInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Path);
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = info.Sized;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Note) + " " + CFunctions.install_keyword(info.Path);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SymbolInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                SymbolInfo info = new SymbolInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Path = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Sized = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, SymbolInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ISymbol Members
        public bool Save(SymbolInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SymbolInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                SymbolInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymbolInfo> Getlist()
        {
            try
            {
                List<SymbolInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " ORDER BY A.orderd DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SymbolInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SymbolInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SymbolInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                SymbolInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymbolInfo> Wcmm_Getlist()
        {
            try
            {
                List<SymbolInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " ORDER BY A.orderd DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SymbolInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SymbolInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SymbolInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<SymbolInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SymbolInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SymbolInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<SymbolInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<SymbolInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SymbolInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SymbolInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CUser
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "user";

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(username, password, name, email, status, markas, timeupdate, pis, pid, depth, logincache, loginfirst, forsearch, id)" +
            " VALUES(@USERNAME, @PASSWORD, @NAME, @EMAIL, @STATUS, @MARKAS, @TIMEUPDATE, @PIS, @PID, @DEPTH, @LOGINCACHE, @LOGINFIRST, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET username=@USERNAME, password=@PASSWORD, name=@NAME, email=@EMAIL, status=@STATUS, markas=@MARKAS, timeupdate=@TIMEUPDATE, pis=@PIS, pid=@PID, depth=@DEPTH, logincache=@LOGINCACHE, loginfirst=@LOGINFIRST, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.username, A.password, A.name, A.email, A.status, A.markas, A.timeupdate, A.pis, A.pid, A.depth, A.logincache, A.loginfirst, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.username, A.password, A.name, A.email, A.status, A.markas, A.timeupdate, A.pis, A.pid, A.depth, A.logincache, A.loginfirst, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_CHANGE_PWD = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET password=@PASSWORD WHERE id=@ID";

        public CUser()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_CHANGE_PWD = SQL_CHANGE_PWD.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PASSWORD = "@PASSWORD";
        private string PARM_NAME = "@NAME";
        private string PARM_EMAIL = "@EMAIL";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_PIS = "@PIS";
        private string PARM_PID = "@PID";
        private string PARM_DEPTH = "@DEPTH";
        private string PARM_LOGINCACHE = "@LOGINCACHE";
        private string PARM_LOGINFIRST = "@LOGINFIRST";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_EMAIL, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_PIS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_PID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_DEPTH, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_LOGINCACHE, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_LOGINFIRST, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, UserInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.SetDBString(info.Password);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Email);
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = info.Pis;
                parms[++i].Value = info.Pid;
                parms[++i].Value = info.Depth;
                parms[++i].Value = info.Logincache;
                parms[++i].Value = info.Loginfirst;
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Username) + " " + CFunctions.install_keyword(info.Email);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private UserInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                UserInfo info = new UserInfo();
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Password = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Email = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Pis = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Pid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Depth = dar.IsDBNull(++i) ? 1 : dar.GetInt32(i);
                info.Logincache = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Loginfirst = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                if (info.iRight == null)
                    info.iRight = (new CUserright()).Getinfo(info.Id);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, UserInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region IUser Members
        public bool Save(UserInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                            {
                                UserrightInfo rinfo = info.iRight;
                                rinfo.Id = info.Id;
                                (new CUserright()).Saveitem(trans, rinfo);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                UserInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserInfo Getinfo(string username)
        {
            if (CFunctions.IsNullOrEmpty(username)) return null;
            try
            {
                UserInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.username=@USERNAME";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = username;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserInfo> Getlist_parent(int pid)
        {
            try
            {
                List<UserInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.id=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (info.Pid != -1)
                                list = this.Getlist_parent(info.Pid);
                            if (list == null)
                                list = new List<UserInfo>();
                            list.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserInfo> Getlist_sub(int pid, List<UserInfo> listin)
        {
            try
            {
                List<UserInfo> list = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (info.Pis != 0)
                                list = this.Getlist_sub(info.Id, list);

                            if (list == null)
                                list = new List<UserInfo>();
                            list.Add(info);
                        }
                        if (listin == null)
                            listin = new List<UserInfo>();
                        if (list != null)
                            listin.AddRange(list);
                    }
                    iConn.Close();
                }
                return listin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CConstants.State.Existed Exist(UserInfo info)
        {
            if (info == null) return CConstants.State.Existed.None;
            try
            {
                CConstants.State.Existed vlreturn = CConstants.State.Existed.None;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    if (HELPER.isExist(iConn, TABLENAME, "username", info.Username, info.Id))
                    {
                        vlreturn = CConstants.State.Existed.Name;
                        goto closeConn;
                    }
                    if (HELPER.isExist(iConn, TABLENAME, "email", info.Email, info.Id))
                    {
                        vlreturn = CConstants.State.Existed.Mail;
                        goto closeConn;
                    }
                closeConn: iConn.Close();
                }
                return vlreturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserInfo Login(UserInfo info)
        {
            try
            {
                UserInfo _info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.pis=0";
                    SQL += " AND (A.username=@USERNAME) AND (A.password=@PASSWORD)";

                    iSqlParameter[] parms = new iSqlParameter[]{
													   new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
													   new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString)
					};
                    parms[0].Value = info.Username;
                    parms[1].Value = info.Password;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            _info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return _info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ChangePwd(UserInfo info)
        {
            try
            {
                if (info == null) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    iSqlParameter[] parms = new iSqlParameter[]{
						new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString),
						new iSqlParameter(PARM_ID, iSqlType.Field_tInterger)
					};
                    parms[0].Value = info.Password;
                    parms[1].Value = info.Id;
                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL_CHANGE_PWD, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserInfo Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                UserInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserInfo Wcmm_Getinfo(string username)
        {
            if (CFunctions.IsNullOrEmpty(username)) return null;
            try
            {
                UserInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.username=@USERNAME";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = username;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserInfo> Wcmm_Getlist(int pid, ListOptions options, out int numResults)
        {
            try
            {
                List<UserInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.pid=" + pid;
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<UserInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, pid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, int pid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += " AND pid=" + pid;

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<UserInfo> Wcmm_Search(int pid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<UserInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += " AND A.pid=" + pid;
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<UserInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, pid, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, int pid, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += " AND pid=" + pid;
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<UserInfo> Wcmm_Getlistgroup(int pid)
        {
            try
            {
                List<UserInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pis>0 AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<UserInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserInfo> Wcmm_Getlistuser(int pid)
        {
            try
            {
                List<UserInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.pis=0";
                    SQL += pid == 0 ? "" : " AND A.pid=" + pid;

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            UserInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<UserInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CUserright
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "userright";

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(r_new, r_upt, r_del, r_sys, id)" +
            " VALUES(@R_NEW, @R_UPT, @R_DEL, @R_SYS, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET r_new=@R_NEW, r_upt=@R_UPT, r_del=@R_DEL, r_sys=@R_SYS WHERE id=@ID";
        private string SQL_GETIFO = "SELECT r_new, r_upt, r_del, r_sys, id FROM " + Queryparam.Varstring.VAR_TABLENAME + " WHERE id<>0";

        public CUserright()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_R_NEW = "@R_NEW";
        private string PARM_R_UPT = "@R_UPT";
        private string PARM_R_DEL = "@R_DEL";
        private string PARM_R_SYS = "@R_SYS";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_R_NEW, iSqlType.Field_tString),
                        new iSqlParameter(PARM_R_UPT, iSqlType.Field_tString),
                        new iSqlParameter(PARM_R_DEL, iSqlType.Field_tString),
                        new iSqlParameter(PARM_R_SYS, iSqlType.Field_tString),
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, UserrightInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.R_new;
                parms[++i].Value = info.R_upt;
                parms[++i].Value = info.R_del;
                parms[++i].Value = info.R_sys;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private UserrightInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                UserrightInfo info = new UserrightInfo();
                info.R_new = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.R_upt = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.R_del = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.R_sys = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Saveitem(iSqlTransaction trans, UserrightInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Deleteitem(iSqlTransaction trans, string iid)
        {
            try
            {
                if (trans == null || CFunctions.IsNullOrEmpty(iid)) return false;

                string SQL = "DELETE FROM " + TABLENAME + " WHERE id IN (" + iid + ")";
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IUserright Members
        public bool Save(UserrightInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserrightInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                UserrightInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Delete(string arr_id)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(arr_id)) return true;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Deleteitem(trans, arr_id);
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CSupportonline
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "supportonline";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, type, note, status, orderd, timeupdate, forsearch, id)" +
            " VALUES(@NAME, @TYPE, @NOTE, @STATUS, @ORDERD, @TIMEUPDATE, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, type=@TYPE, note=@NOTE, status=@STATUS, orderd=@ORDERD, timeupdate=@TIMEUPDATE, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.type, A.note, A.status, A.orderd, A.timeupdate, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.type, A.note, A.status, A.orderd, A.timeupdate, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 ";

        public CSupportonline(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_TYPE = "@TYPE";
        private string PARM_NOTE = "@NOTE";
        private string PARM_STATUS = "@STATUS";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TYPE, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_NOTE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, SupportonlineInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = info.Type;
                parms[++i].Value = CFunctions.SetDBString(info.Note);
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Note);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private SupportonlineInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                SupportonlineInfo info = new SupportonlineInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Type = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Note = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Status = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, SupportonlineInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ISupportonline Members
        public bool Save(SupportonlineInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SupportonlineInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                SupportonlineInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SupportonlineInfo> Getlist()
        {
            try
            {
                List<SupportonlineInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " ORDER BY A.orderd DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SupportonlineInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SupportonlineInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SupportonlineInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                SupportonlineInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<SupportonlineInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<SupportonlineInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SupportonlineInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SupportonlineInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<SupportonlineInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<SupportonlineInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            SupportonlineInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<SupportonlineInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CBanner
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "banner";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, code, album, status, timeupdate, username, forsearch, id)" +
            " VALUES(@NAME, @CODE, @ALBUM, @STATUS, @TIMEUPDATE, @USERNAME, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, code=@CODE, album=@ALBUM, status=@STATUS, timeupdate=@TIMEUPDATE, username=@USERNAME, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.code, A.album, A.status, A.timeupdate, A.username, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.name, A.code, A.album, A.status, A.timeupdate, A.username, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CBanner(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_NAME = "@NAME";
        private string PARM_CODE = "@CODE";
        private string PARM_ALBUM = "@ALBUM";
        private string PARM_STATUS = "@STATUS";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ALBUM, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                        new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, BannerInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = info.Album;
                parms[++i].Value = info.Status;
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Code);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private BannerInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                BannerInfo info = new BannerInfo();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Album = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, BannerInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IBanner Members
        public bool Save(BannerInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            this.Saveitem(trans, info);

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public BannerInfo Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                BannerInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public BannerInfo Getinfo(string code)
        {
            try
            {
                BannerInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.code=@CODE";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = code;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BannerInfo> Getlist()
        {
            try
            {
                List<BannerInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " ORDER BY A.id DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            BannerInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<BannerInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BannerInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                BannerInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BannerInfo> Wcmm_Getlist()
        {
            try
            {
                List<BannerInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " ORDER BY A.id DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            BannerInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<BannerInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<BannerInfo> Wcmm_Getlist(ListOptions options, out int numResults)
        {
            try
            {
                List<BannerInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            BannerInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<BannerInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<BannerInfo> Wcmm_Search(string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<BannerInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            BannerInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<BannerInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, keywords, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string keywords, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                string Searchquery = CGeneral.Get_Searchquery(keywords);
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    public class CVideo
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "video";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(code, name, description, tag, filepreview, cid, orderd, status, markas, iconex, timecreate, timeupdate, username, allowcomment, viewcounter, sourcetype, url, forsearch, id)" +
            " VALUES(@CODE, @NAME, @DESCRIPTION, @TAG, @FILEPREVIEW, @CID, @ORDERD, @STATUS, @MARKAS, @ICONEX, @TIMECREATE, @TIMEUPDATE, @USERNAME, @ALLOWCOMMENT, @VIEWCOUNTER, @SOURCETYPE, @URL, @FORSEARCH, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET code=@CODE, name=@NAME, description=@DESCRIPTION, tag=@TAG, filepreview=@FILEPREVIEW, cid=@CID, orderd=@ORDERD, status=@STATUS, markas=@MARKAS, iconex=@ICONEX, timecreate=@TIMECREATE, timeupdate=@TIMEUPDATE, username=@USERNAME, allowcomment=@ALLOWCOMMENT, viewcounter=@VIEWCOUNTER, sourcetype=@SOURCETYPE, url=@URL, forsearch=@FORSEARCH WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.code, A.name, A.description, A.tag, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.viewcounter, A.sourcetype, A.url, A.id, ROW_NUMBER() OVER(ORDER BY A.id) AS rownumber FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        private string SQL_GETIFOPAGING = "SELECT A.code, A.name, A.description, A.tag, A.filepreview, A.cid, C.name AS cname, A.orderd, A.status, A.markas, A.iconex, A.timecreate, A.timeupdate, A.username, A.allowcomment, A.viewcounter, A.sourcetype, A.url, A.id " + Queryparam.Varstring.VAR_SORTEXPRESSION + " FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + Queryparam.Varstring.VAR_JOINEXPRESSION + " WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;
        private string SQL_COUNT = "SELECT COUNT(A.id) FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A WHERE A.id<>0 AND A.status<>" + (int)CConstants.State.Status.Deleted;

        public CVideo(string lang)
        {
            HELPER = new SqlHelper();

            LANG = lang;
            TABLENAME = lang + TABLENAME;
            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);

            string VAR_JOINEXPRESSION = " LEFT JOIN " + LANG + CConstants.TBDBPREFIX + "category AS C ON C.id=A.cid";
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
            SQL_GETIFOPAGING = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME).Replace(Queryparam.Varstring.VAR_JOINEXPRESSION, VAR_JOINEXPRESSION);
        }

        #region parameters
        private string PARM_ID = "@ID";
        private string PARM_CODE = "@CODE";
        private string PARM_NAME = "@NAME";
        private string PARM_DESCRIPTION = "@DESCRIPTION";
        private string PARM_TAG = "@TAG";
        private string PARM_FILEPREVIEW = "@FILEPREVIEW";
        private string PARM_CID = "@CID";
        private string PARM_ORDERD = "@ORDERD";
        private string PARM_STATUS = "@STATUS";
        private string PARM_MARKAS = "@MARKAS";
        private string PARM_ICONEX = "@ICONEX";
        private string PARM_TIMECREATE = "@TIMECREATE";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_ALLOWCOMMENT = "@ALLOWCOMMENT";
        private string PARM_VIEWCOUNTER = "@VIEWCOUNTER";
        private string PARM_SOURCETYPE = "@SOURCETYPE";
        private string PARM_URL = "@URL";
        private string PARM_FORSEARCH = "@FORSEARCH";
        #endregion

        #region private methods
        private iSqlParameter[] getParameter(string query)
        {
            try
            {
                iSqlParameter[] parms = HELPER.getCachedParameters(query);
                if (parms == null)
                {
                    parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_CODE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_NAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_DESCRIPTION, iSqlType.Field_tText),
					    new iSqlParameter(PARM_TAG, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FILEPREVIEW, iSqlType.Field_tString),
					    new iSqlParameter(PARM_CID, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_ORDERD, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_STATUS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_MARKAS, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_ICONEX, iSqlType.Field_tString),
					    new iSqlParameter(PARM_TIMECREATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
                        new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ALLOWCOMMENT, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_VIEWCOUNTER, iSqlType.Field_tInterger),
                        new iSqlParameter(PARM_SOURCETYPE, iSqlType.Field_tString),
					    new iSqlParameter(PARM_URL, iSqlType.Field_tString),
					    new iSqlParameter(PARM_FORSEARCH, iSqlType.Field_tString),
					    new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
					};
                    HELPER.cacheParameters(query, parms);
                }
                return parms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void setParameter(iSqlParameter[] parms, VideoInfo info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.Code);
                parms[++i].Value = CFunctions.SetDBString(info.Name);
                parms[++i].Value = CFunctions.SetDBString(info.Description);
                parms[++i].Value = CFunctions.SetDBString(info.Tag);
                parms[++i].Value = CFunctions.SetDBString(info.Filepreview);
                parms[++i].Value = info.Cid;
                parms[++i].Value = info.Orderd == 0 ? info.Id : info.Orderd;
                parms[++i].Value = info.Status;
                parms[++i].Value = info.Markas;
                parms[++i].Value = CFunctions.SetDBString(info.Iconex);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timecreate);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = info.Allowcomment;
                parms[++i].Value = info.Viewcounter;
                parms[++i].Value = CFunctions.SetDBString(info.Sourcetype);
                parms[++i].Value = CFunctions.SetDBString(info.Url);
                parms[++i].Value = CFunctions.install_keyword(info.Name) + " " + CFunctions.install_keyword(info.Description) + " " + CFunctions.install_keyword(info.Tag);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private VideoInfo getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                VideoInfo info = new VideoInfo();
                info.Code = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Description = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Tag = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Filepreview = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Cid = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Cname = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Orderd = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Status = dar.IsDBNull(++i) ? (int)CConstants.State.Status.Waitactive : dar.GetInt32(i);
                info.Markas = dar.IsDBNull(++i) ? (int)CConstants.State.MarkAs.None : dar.GetInt32(i);
                info.Iconex = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timecreate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Allowcomment = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Viewcounter = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Sourcetype = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Url = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Rownumber = dar.IsDBNull(++i) ? 0 : dar.GetInt64(i);

                info.lCategory = (new CCategory(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.Video);
                info.lCategoryattr = (new CCategoryattr(LANG)).Wcmm_Getlist_ofiid(info.Id, Webcmm.Id.Video);

                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, VideoInfo info)
        {
            try
            {
                if (trans == null || info == null) return false;
                string SQL = string.Empty;
                if (info.Id == 0)
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else if (HELPER.isExist(trans, TABLENAME, info.Id))
                {
                    SQL = SQL_UPDATE;
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                else
                {
                    SQL = SQL_INSERT;
                    info.Id = (int)HELPER.getNewID(trans, TABLENAME);
                    iSqlParameter[] parms = this.getParameter(SQL);
                    this.setParameter(parms, info);
                    HELPER.executeNonQuery(trans, iCommandType.Text, SQL, parms);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IVideo Members
        public bool Save(VideoInfo info)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (this.Saveitem(trans, info))
                            {
                                if (!CFunctions.IsNullOrEmpty(info.Categoryid))
                                    new CItemcategory(LANG).Save(info.Id, Webcmm.Id.Video, info.Categoryid);
                                if (!CFunctions.IsNullOrEmpty(info.Categoryattrid))
                                    new CItemcategoryattr(LANG).Save(info.Id, Webcmm.Id.Video, info.Categoryattrid);
                            }

                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VideoInfo Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                VideoInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VideoInfo> Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<VideoInfo> Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<VideoInfo> Getlistrelated(string relateditem)
        {
            if (CFunctions.IsNullOrEmpty(relateditem)) return null;
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += " AND A.id IN(" + relateditem + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VideoInfo> Getlistattr(string categoryid, int attrid, string attrcode, ListOptions options, out int numResults)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                    SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                    SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    SQL += attrid == 0 ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    SQL += CFunctions.IsNullOrEmpty(attrcode) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr AS ICA INNER JOIN " + LANG + CConstants.TBDBPREFIX + "categoryattr AS CA ON ICA.categoryid=CA.id WHERE CA.code='" + attrcode + "' AND ICA.belongto=" + Webcmm.Id.Video + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                        dar.Close();
                    }
                    numResults = this.Getlistcountattr(iConn, categoryid, attrid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Getlistcountattr(iSqlConnection iConn, string categoryid, int attrid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Waitactive;
                SQL += " AND A.status<>" + (int)CConstants.State.Status.Disabled;
                SQL += options.Markas == (int)CConstants.State.MarkAs.None ? "" : " AND A.markas=" + options.Markas;
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                SQL += attrid == 0 ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategoryattr WHERE categoryid IN(" + attrid + ") AND belongto=" + Webcmm.Id.Video + ")";
                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public VideoInfo Wcmm_Getinfo(int id)
        {
            if (id == 0) return null;
            try
            {
                VideoInfo info = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO + " AND A.id=@ID";

                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter(PARM_ID, iSqlType.Field_tInterger),
                    };
                    int i = -1;
                    parms[++i].Value = id;
                    using (iSqlDataReader dar = HELPER.executeReader(iConn, iCommandType.Text, SQL, parms))
                    {
                        if (dar.Read())
                        {
                            info = this.getDataReader(dar);
                        }
                    }
                    iConn.Close();
                }
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VideoInfo> Wcmm_Getlist(string iid)
        {
            if (CFunctions.IsNullOrEmpty(iid)) return null;
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += CFunctions.IsNullOrEmpty(iid) ? "" : " AND A.id IN(" + iid + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VideoInfo> Wcmm_Getlist(int cid)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += cid == 0 ? "" : " AND A.cid IN(" + (new CCategory(LANG)).Get_setof(cid, "") + ")";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                    }
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<VideoInfo> Wcmm_Getlist(string categoryid, ListOptions options, out int numResults)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Getlistcount(iConn, categoryid, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Getlistcount(iSqlConnection iConn, string categoryid, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }
        public List<VideoInfo> Wcmm_Search(string categoryid, string keywords, ListOptions options, out int numResults)
        {
            try
            {
                List<VideoInfo> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFOPAGING.Replace(Queryparam.Varstring.VAR_SORTEXPRESSION, CFunctions.Expression_GetSort(options.SortExp, options.SortDir));
                    SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                    SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                    string Searchquery = CGeneral.Get_Searchquery(keywords);
                    SQL += CFunctions.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";
                    SQL = "SELECT * FROM(" + SQL + ") AS T WHERE id<>0 " + CFunctions.Expression_GetLimit(options.PageIndex, options.PageSize);

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            VideoInfo info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<VideoInfo>();
                            arr.Add(info);
                        }
                    }
                    numResults = this.Wcmm_Searchcount(iConn, categoryid, Searchquery, options);
                    iConn.Close();
                }
                return arr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int Wcmm_Searchcount(iSqlConnection iConn, string categoryid, string Searchquery, ListOptions options)
        {
            try
            {
                int numResults = 0;
                string SQL = SQL_COUNT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
                SQL += CFunctions.Expression_GetPermit(options.GetAll, options.Username);
                SQL += CFunctions.IsNullOrEmpty(categoryid) ? "" : " AND A.id IN(SELECT iid FROM " + LANG + CConstants.TBDBPREFIX + "itemcategory WHERE categoryid IN(" + categoryid + ") AND belongto=" + Webcmm.Id.Video + ")";
                SQL += string.IsNullOrEmpty(Searchquery) ? "" : " AND (" + Searchquery + ")";

                using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                {
                    if (dar.Read())
                    {
                        numResults = dar.IsDBNull(0) ? 0 : dar.GetInt32(0);
                    }
                }
                return numResults;
            }
            catch
            {
                return 0;
            }
        }

        public bool Updatenum(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string valueset = value.ToString();
                    if (valueset == CConstants.NUM_INCREASE.ToString())
                        valueset = column + "+1";
                    else if (valueset == CConstants.NUM_DECREASE.ToString())
                        valueset = column + "-1";
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=" + valueset + " WHERE id IN(" + id + ")";
                    HELPER.executeNonQuery(iConn, SQL);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool Updatestr(string id, string column, object value)
        {
            try
            {
                if (CFunctions.IsNullOrEmpty(id)) return false;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = "UPDATE " + TABLENAME + " SET " + column + "=@PARM_VALUE WHERE id IN(" + id + ")";
                    iSqlParameter[] parms = new iSqlParameter[]{
                        new iSqlParameter("PARM_VALUE", iSqlType.Field_tString),
                    };
                    int i = -1;
                    parms[++i].Value = value.ToString();

                    HELPER.executeNonQuery(iConn, iCommandType.Text, SQL, parms);
                    iConn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

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
    public class CMailServer
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "settingsite_mailserver";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(smtpserver, smtpport, usessl, receiver, username, password, timeupdate, id)" +
            " VALUES(@SMTPSERVER, @SMTPPORT, @USESSL, @RECEIVER, @USERNAME, @PASSWORD, @TIMEUPDATE, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET smtpserver=@SMTPSERVER, smtpport=@SMTPPORT, usessl=@USESSL, receiver=@RECEIVER, username=@USERNAME, password=@PASSWORD, timeupdate=@TIMEUPDATE WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.smtpserver, A.smtpport, A.usessl, A.receiver, A.username, A.password, A.timeupdate, A.id FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + " WHERE A.id<>0";

        public CMailServer()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CMailServer(string lang)
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
        private string PARM_SMTPSERVER = "@SMTPSERVER";
        private string PARM_SMTPPORT = "@SMTPPORT";
        private string PARM_USESSL = "@USESSL";
        private string PARM_RECEIVER = "@RECEIVER";
        private string PARM_USERNAME = "@USERNAME";
        private string PARM_PASSWORD = "@PASSWORD";
        private string PARM_TIMEUPDATE = "@TIMEUPDATE";
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
                        new iSqlParameter(PARM_SMTPSERVER, iSqlType.Field_tString),
					    new iSqlParameter(PARM_SMTPPORT, iSqlType.Field_tInterger),							
					    new iSqlParameter(PARM_USESSL, iSqlType.Field_tInterger),
					    new iSqlParameter(PARM_RECEIVER, iSqlType.Field_tString),
					    new iSqlParameter(PARM_USERNAME, iSqlType.Field_tString),
                        new iSqlParameter(PARM_PASSWORD, iSqlType.Field_tString),
                        new iSqlParameter(PARM_TIMEUPDATE, iSqlType.Field_tDate),
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
        private void setParameter(iSqlParameter[] parms, Settingsite.MailServer info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = CFunctions.SetDBString(info.SMTPServer);
                parms[++i].Value = info.SMTPPort;
                parms[++i].Value = info.UseSSL;
                parms[++i].Value = CFunctions.SetDBString(info.Receiver);
                parms[++i].Value = CFunctions.SetDBString(info.Username);
                parms[++i].Value = CFunctions.SetDBString(info.Password);
                parms[++i].Value = CFunctions.SetDBDatetime(info.Timeupdate);
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Settingsite.MailServer getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                Settingsite.MailServer info = new Settingsite.MailServer();
                info.SMTPServer = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.SMTPPort = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.UseSSL = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                info.Receiver = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Username = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Password = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Timeupdate = dar.IsDBNull(++i) ? new DateTime(0) : dar.GetDateTime(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, Settingsite.MailServer info)
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

        #region IMail Members
        public bool Save(Settingsite.MailServer info)
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
        public Settingsite.MailServer Getinfo(int id)
        {
            try
            {
                Settingsite.MailServer info = null;
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

        #region public methods
        public bool SendMail(string emailfr, string subject, string content)
        {
            try
            {
                //Settingsite.MailServer info = this.Getinfo(1);
                //if (info == null) return false;

                //if (info.UseGMAIL != 0)
                //{
                //    RC.Gmail.GmailMessage gmailMsg = new RC.Gmail.GmailMessage(info.Username, info.Password);
                //    gmailMsg.From = CFunctions.IsNullOrEmpty(emailfr) ? info.Sender : emailfr;
                //    gmailMsg.To = info.Receiver;
                //    gmailMsg.Subject = subject;
                //    gmailMsg.Body = content;
                //    gmailMsg.BodyEncoding = System.Text.Encoding.UTF8;
                //    gmailMsg.BodyFormat = MailFormat.Html;
                //    gmailMsg.Send();
                //}
                //else
                //{
                //    MailMessage mm = new MailMessage();
                //    mm.Subject = subject;
                //    mm.Body = content;
                //    mm.From = CFunctions.IsNullOrEmpty(emailfr) ? info.Sender : emailfr;
                //    mm.To = info.Receiver;
                //    mm.BodyEncoding = System.Text.Encoding.UTF8;
                //    mm.BodyFormat = MailFormat.Html;

                //    SmtpMail.SmtpServer = info.SMTPServer;
                //    SmtpMail.Send(mm);

                //}
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SendMail(string emailfr, string emailto, string subject, string content)
        {
            try
            {
                //Settingsite.MailServer info = this.Getinfo(1);
                //if (info == null) return false;

                //if (info.UseGMAIL != 0)
                //{
                //    RC.Gmail.GmailMessage gmailMsg = new RC.Gmail.GmailMessage(info.Username, info.Password);
                //    gmailMsg.From = CFunctions.IsNullOrEmpty(emailfr) ? info.Sender : emailfr;
                //    gmailMsg.To = CFunctions.IsNullOrEmpty(emailto) ? info.Receiver : emailto;
                //    gmailMsg.Subject = subject;
                //    gmailMsg.Body = content;
                //    gmailMsg.BodyEncoding = System.Text.Encoding.UTF8;
                //    gmailMsg.BodyFormat = MailFormat.Html;
                //    gmailMsg.Send();
                //}
                //else
                //{
                //    MailMessage mm = new MailMessage();
                //    mm.Subject = subject;
                //    mm.Body = content;
                //    mm.From = CFunctions.IsNullOrEmpty(emailfr) ? info.Sender : emailfr;
                //    mm.To = CFunctions.IsNullOrEmpty(emailto) ? info.Receiver : emailto;
                //    mm.BodyEncoding = System.Text.Encoding.UTF8;
                //    mm.BodyFormat = MailFormat.Html;

                //    SmtpMail.SmtpServer = info.SMTPServer;
                //    SmtpMail.Send(mm);
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }

    public class CRestrictedPages
    {
        private SqlHelper HELPER = null;
        private string TABLENAME = CConstants.TBDBPREFIX + "settingsite_restrictedpages";
        private string LANG;

        private string SQL_INSERT = "INSERT INTO " + Queryparam.Varstring.VAR_TABLENAME + "(name, query, pathandquery, id)" +
            " VALUES(@NAME, @QUERY, @PATHANDQUERY, @ID)";
        private string SQL_UPDATE = "UPDATE " + Queryparam.Varstring.VAR_TABLENAME + " SET name=@NAME, query=@QUERY, pathandquery=@PATHANDQUERY WHERE id=@ID";
        private string SQL_GETIFO = "SELECT A.name, A.query, A.pathandquery, A.id FROM " + Queryparam.Varstring.VAR_TABLENAME + " AS A " + " WHERE A.id<>0";

        public CRestrictedPages()
        {
            HELPER = new SqlHelper();

            SQL_INSERT = SQL_INSERT.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_UPDATE = SQL_UPDATE.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
            SQL_GETIFO = SQL_GETIFO.Replace(Queryparam.Varstring.VAR_TABLENAME, TABLENAME);
        }
        public CRestrictedPages(string lang)
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
        private string PARM_NAME = "@NAME";
        private string PARM_QUERY = "@QUERY";
        private string PARM_PATHANDQUERY = "@PATHANDQUERY";
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
					    new iSqlParameter(PARM_QUERY, iSqlType.Field_tString),							
					    new iSqlParameter(PARM_PATHANDQUERY, iSqlType.Field_tString),							
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
        private void setParameter(iSqlParameter[] parms, Settingsite.RestrictedPages info)
        {
            try
            {
                int i = -1;
                parms[++i].Value = info.Name;
                parms[++i].Value = info.Query;
                parms[++i].Value = info.PathandQuery;
                parms[++i].Value = info.Id;
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Settingsite.RestrictedPages getDataReader(iSqlDataReader dar)
        {
            try
            {
                int i = -1;
                Settingsite.RestrictedPages info = new Settingsite.RestrictedPages();
                info.Name = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Query = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.PathandQuery = dar.IsDBNull(++i) ? string.Empty : dar.GetString(i);
                info.Id = dar.IsDBNull(++i) ? 0 : dar.GetInt32(i);
                
                return info;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Saveitem(iSqlTransaction trans, Settingsite.RestrictedPages info)
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
        private bool Deleteitem(iSqlTransaction trans)
        {
            try
            {
                if (trans == null) return false;

                string SQL = "DELETE FROM " + TABLENAME;
                HELPER.executeNonQuery(trans, SQL);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion

        #region INotice Members
        public bool Save(Settingsite.RestrictedPages info)
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
        public bool Save(List<Settingsite.RestrictedPages> list)
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            if (list != null && list.Count > 0)
                                foreach (Settingsite.RestrictedPages info in list)
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
        public Settingsite.RestrictedPages Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                Settingsite.RestrictedPages info = null;
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
        public List<Settingsite.RestrictedPages> Getlist(string pagename)
        {
            try
            {
                List<Settingsite.RestrictedPages> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " AND A.name='" + HELPER.sqlEscape(pagename) + "'";
                    SQL += " ORDER BY A.id DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            Settingsite.RestrictedPages info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<Settingsite.RestrictedPages>();
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
        
        public Settingsite.RestrictedPages Wcmm_Getinfo(int id)
        {
            if (id == -1) return null;
            try
            {
                Settingsite.RestrictedPages info = null;
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
        public List<Settingsite.RestrictedPages> Wcmm_Getlist()
        {
            try
            {
                List<Settingsite.RestrictedPages> arr = null;
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    string SQL = SQL_GETIFO;
                    SQL += " ORDER BY A.id DESC";

                    using (iSqlDataReader dar = HELPER.executeReader(iConn, SQL))
                    {
                        while (dar.Read())
                        {
                            Settingsite.RestrictedPages info = this.getDataReader(dar);
                            if (arr == null)
                                arr = new List<Settingsite.RestrictedPages>();
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
        public bool Wcmm_Deleteall()
        {
            try
            {
                using (iSqlConnection iConn = HELPER.getConnect(HELPER.SQL_SYSTEM))
                {
                    using (iSqlTransaction trans = iConn.BeginTransaction())
                    {
                        try
                        {
                            string SQL = "DELETE FROM " + TABLENAME;
                            HELPER.executeNonQuery(trans, SQL);

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
}

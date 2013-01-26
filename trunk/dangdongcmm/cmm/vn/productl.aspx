<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="productl.aspx.cs" Inherits="dangdongcmm.cmm.productl" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Setupattribute" Src="usercontrol/ucsetupattribute.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td><img src="/cmm/images/symsearch.gif" alt="" /></td>
                                <td><ASP:DROPDOWNLIST ID="ddlCid" runat="server" onchange="ddlGotopage(this, 'productl.aspx');"></ASP:DROPDOWNLIST></td>
                                <td><ASP:TEXTBOX ID="txtKeyword" runat="server"></ASP:TEXTBOX></td>
                                <td><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Search" /></td>
                            </tr>
                        </table>
                    </ASP:PANEL>
                </div>
                <div class="colsetting">
                    <ASP:BUTTON ID="cmdAdd" runat="server" Text="Thêm mới" CssClass="buttoncmd" />&nbsp;
                    -- hoặc --
                    <ASP:DROPDOWNLIST ID="ddlAction" runat="server" onchange="actionMulti(this);" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                        <ASP:LISTITEM Value="" class="textdefndis">Thiết lập nhiều sản phẩm...</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Delete">-- Xóa</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Copy">-- Sao chép</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Move">-- Di chuyển</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Updatedisplayorder">-- Cập nhật thứ tự hiển thị</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Setupattribute">-- Cài đặt hiển thị</ASP:LISTITEM>
                    </ASP:DROPDOWNLIST>
                </div>  
            </div>
            
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting" OnRowDeleting="grdView_RowDeleting" OnRowCommand="grdView_RowCommand">
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            <%# Eval("Rownumber")%>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <input type="checkbox" id="chkcheck<%# Eval("Id")%>" name="chkcheck" value="<%# Eval("Id")%>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên sản phẩm" sortexpression="Name">
                        <ITEMTEMPLATE>
                            <%# Eval("eMarkas")%> <ASP:LINKBUTTON ID="cmdSetupattribute" runat="server" CommandName="Setupattribute" CommandArgument='<%# Eval("Id")%>' Text='<%#Eval("Name") %>' CausesValidation="false"></ASP:LINKBUTTON> <%#Eval("Iconex").ToString() == "" ? "" : "<img src='../../" + Eval("Iconex") + "' />" %>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="Code" headertext="Mã SKU" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD headertext="Giá" sortexpression="Price" itemstyle-cssclass="colprice">
                        <ITEMTEMPLATE>
                            $<%#Eval("ePrice") %> &nbsp; &nbsp;
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Đánh giá" itemstyle-cssclass="colstatus">
                        <ITEMTEMPLATE>
                            <a href='commentl.aspx?blto=10&cid=<%=Cid %>&pid=<%#Eval("Id") %>'><%#Eval("Allowcomment").ToString() == "0" ? "" : ("Comment (" + (int.Parse(Eval("Allowcomment").ToString()) - 1) + ")")%></a>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Thứ tự" sortexpression="Orderd" itemstyle-cssclass="colorderd">
                        <ITEMTEMPLATE>
                            <ASP:TEXTBOX ID="txtOrderd" runat="server" CssClass="ingrdinput" Text='<%#Eval("Orderd") %>' onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this)"></ASP:TEXTBOX>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="eStatus" headertext="Trạng thái" sortexpression="Status" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colaction">
                        <ITEMTEMPLATE>
                            <a class="command" href="javascript:CC_gotoUrl('productu.aspx?cid=<%=Cid %>&pag=<%=PageIndex %>&iid=<%#Eval("Id") %>');">Sửa</a>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="cmdDeleteone" runat="server" CssClass="command" Text="Xóa" CommandName="Delete" CommandArgument='<%# Eval("Id")%>' CausesValidation="false"></ASP:LINKBUTTON>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                </COLUMNS>
                <HEADERSTYLE cssclass="rowheader" />
                <ROWSTYLE cssclass="rowormal" />
                <ALTERNATINGROWSTYLE cssclass="rowalter" />
                
            </ASP:GRIDVIEW>
            <div align="right" class="pagercontainer">
		        <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" />
            </div>
            </div>
            <UC:SETUPATTRIBUTE ID="Setupattribute" runat="server" />
        </ASP:PANEL>

        <ASP:HIDDENFIELD ID="txtIidstr" runat="server" />
        <ASP:PANEL id="pnlCopy" runat="server" Visible="false" CssClass="tformin tformincom">
            <div class="rowhor"><b>Các sản phẩm sau sẽ được sao chép:</b></div>
            <div class="rowhor">
                <ASP:GRIDVIEW ID="grdCopy" runat="server" CssClass="lgridnoborder"
                    AutoGenerateColumns="false" AllowSorting="false" ShowHeader="false">
                    <COLUMNS>
                        <ASP:TEMPLATEFIELD ItemStyle-CssClass="colstatus">
                            <ITEMTEMPLATE>
                                <li>[ <%#Eval("Id") %> ] &nbsp; 
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                        <ASP:TEMPLATEFIELD>
                            <ITEMTEMPLATE>
                                <a><%#Eval("Name") %></a>
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                    </COLUMNS>
                    <ROWSTYLE CssClass="rowormal" />
                </ASP:GRIDVIEW>
            </div>
            <div class="rowhor"><b>Sao chép qua:</b></div>
            <div class="rowhor">
                <div class="boxctrlc">Danh mục</div>
            </div>
            <div class="rowhor">
                <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCopycid" runat="server"></ASP:DROPDOWNLIST></div>
            </div>
            <br />&nbsp;
            <div class="rowhor">
                <fieldset>
                    <legend>
                        <ASP:BUTTON ID="cmdCopyOk" runat="server" CssClass="button" Text="Sao chép" OnClientClick="javascript:return doSubmit();" OnClick="cmdCopyOk_Click" />
                        <input type="button" class="button" value="Quay lại" onclick="javascript:location.href=location.href;" />
                    </legend>
                </fieldset>
            </div>
        </ASP:PANEL>

        <ASP:PANEL id="pnlMove" runat="server" Visible="false" CssClass="tformin tformincom">
            <div class="rowhor"><b>Các sản phẩm sau sẽ được di chuyển:</b></div>
            <div class="rowhor">
                <ASP:GRIDVIEW ID="grdMove" runat="server" CssClass="lgridnoborder"
                    AutoGenerateColumns="false" AllowSorting="false" ShowHeader="false">
                    <COLUMNS>
                        <ASP:TEMPLATEFIELD ItemStyle-CssClass="colstatus">
                            <ITEMTEMPLATE>
                                <li>[ <%#Eval("Id") %> ] &nbsp; 
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                        <ASP:TEMPLATEFIELD>
                            <ITEMTEMPLATE>
                                <a><%#Eval("Name") %></a>
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                    </COLUMNS>
                    <ROWSTYLE CssClass="rowormal" />
                </ASP:GRIDVIEW>
            </div>
            <div class="rowhor"><b>Di chuyển qua:</b></div>
            <div class="rowhor">
                <div class="boxctrlc">Danh mục</div>
            </div>
            <div class="rowhor">
                <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlMovecid" runat="server"></ASP:DROPDOWNLIST></div>
            </div>
            <br />&nbsp;
            <div class="rowhor">
                <fieldset>
                    <legend>
                        <ASP:BUTTON ID="cmdMoveOk" runat="server" CssClass="button" Text="Di chuyển" OnClientClick="javascript:return doSubmit();" OnClick="cmdMoveOk_Click" />
                        <input type="button" class="button" value="Quay lại" onclick="javascript:location.href=location.href;" />
                    </legend>
                </fieldset>
            </div>
        </ASP:PANEL>
                
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucrelatednews.ascx.cs" Inherits="dangdongcmm.cmm.ucrelatednews" %>

<ASP:PANEL ID="pnlRelated" runat="server" Visible="true" CssClass="rowhor">
    <div class="title">
    <div class="dformoutcommand">
        <div class="colfilter" style="width:100%">
            <div><ASP:DROPDOWNLIST ID="RN_ddlCid" runat="server" onchange="RNloadlist(this)"></ASP:DROPDOWNLIST></div>
            <div style="margin-top:10px;"><select id="RN_listviewNews" multiple="multiple" style="height:250px"></select></div>
            <div style="margin-top:10px;"><input type="button" value=" Thêm vào Danh Sách Liên Quan " onclick="RNgetlist()" class="buttoncmd" /></div>
        </div>
    </div>
    </div>
    <div class="boxctrll">
        <div id="RN_error"></div>
        <table class="RN_listselectedNews">
            <tr class="rnrowi">
                <th class="rnnote">Tiêu đề</th><th class="rnsort">Danh mục</th><th class="rncomd">Xóa</th>
            </tr>
        </table>
        <div id="RN_containerNews" runat="server" title="0">
            <table id="RN_listselectedNews" class="RN_listselectedNews"></table>
        </div>
    </div>
    <ASP:HIDDENFIELD ID="RN_txtRelatedid" runat="server" />
    
    <script type="text/javascript">
    function RNloadlist(e) {
        $(e).upload('../RNLoadHandler.ashx?cid=' + $(e).val(), function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) { $('#RN_listviewNews').children().remove(); return; }
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            $('#RN_listviewNews').children().remove();
            for(var i=0; i < json.newsinfo.length; i++) {
                $('<option value="' + json.newsinfo[i].newsid + '">' + json.newsinfo[i].newsname + '</option>').appendTo('#RN_listviewNews');
            }
        }, 'html');
    }
    function RNgetlist() {
        var selectedId = RNgetselectedid();
        $('#RN_listviewNews').upload('../RNLoadHandler.ashx?iid=' + selectedId, function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) return;
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            for(var i=0; i < json.newsinfo.length; i++) {
                var row = $('<tr id="rnrowi'+FAindex+'" class="rnrowi"></tr>').appendTo('#RN_listselectedNews');
                $('<td class="rnnote">' + json.newsinfo[i].newsname + '</td>').appendTo(row);
                $('<td class="rnsort">' + json.newsinfo[i].newscategory + '</td>').appendTo(row);
                $('<td class="rncomd"><a href="javascript:RNremove(' + json.newsinfo[i].newsid + ',' + FAindex + ');">x</a></td>').appendTo(row);
                FAindex++;
            }
            $('#RN_error').html('Bạn cần nhấn nút "Lưu" để lưu lại danh sách này.');
            RNchanged = true;
        }, 'html');
    }
    function RNgetselectedid() {
        var relatedId = $('#RN_listviewNews').val().toString();
        var selectedId = $('#<%=RN_txtRelatedid.ClientID %>').val();
        var newselectedId = '';
        if( !VALIDATA.IsNullOrEmpty(selectedId) ) {
            var strrelatedId = $('#<%=RN_txtRelatedid.ClientID %>').val();
            var arrrelatedId = relatedId.split(',');
            for(var i=0; i<arrrelatedId.length; i++) {
                if((',' + strrelatedId + ',').indexOf(',' + arrrelatedId[i] + ',') == -1) {
                    selectedId += ',' + arrrelatedId[i];
                    newselectedId += (newselectedId=='' ? '' : ',') + arrrelatedId[i];
                }    
            }
        }
        else {
            selectedId = relatedId;
            newselectedId = relatedId;
        }
        $('#<%=RN_txtRelatedid.ClientID %>').val(selectedId);
        return newselectedId;
    }
    
    var FAindex = Math.abs($('#<%=RN_containerNews.ClientID %>').attr('title'));
    var RNchanged = false;
    function RNremove(newsid, index) {
        var strrelatedId = (',' + $('#<%=RN_txtRelatedid.ClientID %>').val() + ',').replace(',' + newsid + ',', ',');
        $('#<%=RN_txtRelatedid.ClientID %>').val(strrelatedId == ',' ? '' : strrelatedId.substring(1, strrelatedId.length - 1));
        $('#RN_listselectedNews').find('#rnrowi' + index).remove();
        $('#RN_error').html('Bạn cần nhấn nút "Lưu" để hoàn tất việc xóa tin liên quan.');
        RNchanged = true;
    }
    </script>
</ASP:PANEL>
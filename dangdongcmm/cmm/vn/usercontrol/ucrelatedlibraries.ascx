<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucrelatedlibraries.ascx.cs" Inherits="dangdongcmm.cmm.ucrelatedlibraries" %>

<ASP:PANEL ID="pnlRelated" runat="server" Visible="true" CssClass="rowhor">
    <div class="title">
    <div class="dformoutcommand">
        <div class="colfilter" style="width:100%">
            <div><ASP:DROPDOWNLIST ID="RL_ddlCid" runat="server" onchange="RLloadlist(this)"></ASP:DROPDOWNLIST></div>
            <div style="margin-top:10px;"><select id="RL_listviewLibraries" multiple="multiple" style="height:250px"></select></div>
            <div style="margin-top:10px;"><input type="button" value=" Thêm vào Danh Sách Liên Quan " onclick="RLgetlist()" class="buttoncmd" /></div>
        </div>
    </div>
    </div>
    <div class="boxctrll">
        <div id="RL_error"></div>
        <table class="RL_listselectedLibraries">
            <tr class="rlrowi">
                <th class="rlnote">Tiêu đề</th><th class="rlsort">Danh mục</th><th class="rlcomd">Xóa</th>
            </tr>
        </table>
        <div id="RL_containerLibraries" runat="server" title="0">
            <table id="RL_listselectedLibraries" class="RL_listselectedLibraries"></table>
        </div>
    </div>
    <ASP:HIDDENFIELD ID="RL_txtRelatedid" runat="server" />
    
    <script type="text/javascript">
    function RLloadlist(e) {
        $(e).upload('../RLLoadHandler.ashx?cid=' + $(e).val(), function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) { $('#RL_listviewLibraries').children().remove(); return; }
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            $('#RL_listviewLibraries').children().remove();
            for(var i=0; i < json.librariesinfo.length; i++) {
                $('<option value="' + json.librariesinfo[i].librariesid + '">' + json.librariesinfo[i].librariesname + '</option>').appendTo('#RL_listviewLibraries');
            }
        }, 'html');
    }
    function RLgetlist() {
        var selectedId = RLgetselectedid();
        $('#RL_listviewLibraries').upload('../RLLoadHandler.ashx?iid=' + selectedId, function(res) {
            if(VALIDATA.IsNullOrEmpty(res)) return;
            var json = eval("(" + res.substring(5, res.length - 6) + ")");
            for(var i=0; i < json.librariesinfo.length; i++) {
                var row = $('<tr id="rlrowi'+FAindex+'" class="rlrowi"></tr>').appendTo('#RL_listselectedLibraries');
                $('<td class="rlnote">' + json.librariesinfo[i].librariesname + '</td>').appendTo(row);
                $('<td class="rlsort">' + json.librariesinfo[i].librariescategory + '</td>').appendTo(row);
                $('<td class="rlcomd"><a href="javascript:RLremove(' + json.librariesinfo[i].librariesid + ',' + FAindex + ');">x</a></td>').appendTo(row);
                FAindex++;
            }
            $('#RL_error').html('Bạn cần nhấn nút "Lưu" để lưu lại danh sách này.');
            RLchanged = true;
        }, 'html');
    }
    function RLgetselectedid() {
        var relatedId = $('#RL_listviewLibraries').val().toString();
        var selectedId = $('#<%=RL_txtRelatedid.ClientID %>').val();
        var librarieselectedId = '';
        if( !VALIDATA.IsNullOrEmpty(selectedId) ) {
            var strrelatedId = $('#<%=RL_txtRelatedid.ClientID %>').val();
            var arrrelatedId = relatedId.split(',');
            for(var i=0; i<arrrelatedId.length; i++) {
                if((',' + strrelatedId + ',').indexOf(',' + arrrelatedId[i] + ',') == -1) {
                    selectedId += ',' + arrrelatedId[i];
                    librarieselectedId += (librarieselectedId=='' ? '' : ',') + arrrelatedId[i];
                }    
            }
        }
        else {
            selectedId = relatedId;
            librarieselectedId = relatedId;
        }
        $('#<%=RL_txtRelatedid.ClientID %>').val(selectedId);
        return librarieselectedId;
    }
    
    var FAindex = Math.abs($('#<%=RL_containerLibraries.ClientID %>').attr('title'));
    var RLchanged = false;
    function RLremove(librariesid, index) {
        var strrelatedId = (',' + $('#<%=RL_txtRelatedid.ClientID %>').val() + ',').replace(',' + librariesid + ',', ',');
        $('#<%=RL_txtRelatedid.ClientID %>').val(strrelatedId == ',' ? '' : strrelatedId.substring(1, strrelatedId.length - 1));
        $('#RL_listselectedLibraries').find('#rlrowi' + index).remove();
        $('#RL_error').html('Bạn cần nhấn nút "Lưu" để hoàn tất việc xóa tin liên quan.');
        RLchanged = true;
    }
    </script>
</ASP:PANEL>
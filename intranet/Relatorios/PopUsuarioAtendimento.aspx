<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopUsuarioAtendimento.aspx.cs" Inherits="eVidaIntranet.Relatorios.PopUsuarioAtendimento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function ConfigControlsCustom() {
            $("[id*=chkAll]").live("click", function () {
                var chkHeader = $(this);
                var grid = $(this).closest("table");
                $("input[type=checkbox]", grid).each(function () {
                    if (chkHeader.is(":checked")) {
                        $(this).attr("checked", "checked");
                        //$("td", $(this).closest("tr")).addClass("selected");
                    } else {
                        $(this).removeAttr("checked");
                       // $("td", $(this).closest("tr")).removeClass("selected");
                    }
                });
            });

            $("[id*=chkSingle]").live("click", function () {
                var chk = $(this);
                if (!chk.is(":checked")) {
                    $("[id*=chkAll]").attr("checked", false);
                }
            });
        }

        function addUsuario(id, nome) {
            parent.addUsuario(id, nome);
        }

        function addUsuarios(ids) {
            parent.addUsuarios(ids);
        }

        function changeChkAll(val) {
            var ok = val;
            alert(ok);
            $('.chk').each(function () {
                //alert(this.id + " - " + ok);
                $(this).children(0).checked = ok;
                //this.checked = ok;
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="450px">
        <tr>
            <td><asp:Label ID="lblNome" runat="server" Text="Login:" /></td>
            <td><asp:TextBox ID="txtNome" runat="server" Width="250px" /></td>
            <td><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <asp:Button ID="btnSelecao" runat="server" OnClick="btnSelecao_Click" Text="Enviar múltipla seleção" Visible="false" />
                <asp:GridView ID="gdvUsuario" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="CD_USUARIO" AutoGenerateColumns="false"
                    OnRowCommand="gdvUsuario_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <input type="checkbox" id="chkAll" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSingle" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:ButtonField ButtonType="Link" Text="Sel. única" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="cd_usuario" HeaderText="USUÁRIO" />
                        <asp:BoundField DataField="nm_usuario" HeaderText="NOME" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>

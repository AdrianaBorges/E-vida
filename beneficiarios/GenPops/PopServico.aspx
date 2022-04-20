<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopServico.aspx.cs" Inherits="eVidaBeneficiarios.GenPops.PopServico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setServico(cd_servico) {
            parent.locatorCallback(cd_servico);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="450px">
        <tr>
            <td><asp:Label ID="lblCodigo" runat="server" Text="Código:" /></td>
            <td><asp:TextBox ID="txtCodigo" runat="server" Width="250px" /></td>
            <td rowspan="2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblDescricao" runat="server" Text="Descrição:" /></td>
            <td><asp:TextBox ID="txtDescricao" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gdv" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="CD_SERVICO" AutoGenerateColumns="false"
                    OnRowCommand="gdv_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="BR8_CODPSA" HeaderText="CÓDIGO" />
						<asp:BoundField DataField="BR8_CODPAD" HeaderText="TABELA" />
                        <asp:BoundField DataField="BR8_DESCRI" HeaderText="DESCRIÇÃO" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>

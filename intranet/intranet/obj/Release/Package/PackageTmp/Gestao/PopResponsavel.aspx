<%@ Page Title="" Language="C#" MasterPageFile="~/Internal/InternalPopup.Master" AutoEventWireup="true" CodeBehind="PopResponsavel.aspx.cs" Inherits="eVidaIntranet.Gestao.PopResponsavel" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function setResponsavel(cdBenef, cdFuncionario, nome) {
            var resp = new Object();
            resp.cdBeneficiario = cdBenef;
            resp.cdFuncionario = cdFuncionario;
            resp.nmFuncionario = nome;
            //parent.setResponsavel(resp);
            parent.locatorCallback(resp);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="530px">
        <tr>
            <td><asp:Label ID="lblMatricula" runat="server" Text="Matrícula:" /></td>
            <td><asp:TextBox ID="txtMatricula" runat="server" Width="150px" /></td>
            <td rowspan="2"><asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" /></td>
        </tr>
        <tr>
            <td><asp:Label ID="lblNome" runat="server" Text="Nome:" /></td>
            <td><asp:TextBox ID="txtNome" runat="server" Width="250px" /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gdvResponsavel" runat="server" Width="100%" CssClass="tabela"
                    DataKeyNames="CD_FUNCIONARIO" AutoGenerateColumns="false"
                    OnRowCommand="gdvResponsavel_RowCommand">
                    <Columns>
                        <asp:ButtonField ButtonType="Link" Text="Selecionar" CommandName="CmdSelecionar"  />
                        <asp:BoundField DataField="CD_FUNCIONARIO" HeaderText="MATRÍCULA" />
                        <asp:BoundField DataField="CD_BENEFICIARIO" HeaderText="COD BENEFICIÁRIO" />
                        <asp:BoundField DataField="nm_BENEFICIARIO" HeaderText="NOME" />
                    </Columns>
                </asp:GridView>

            </td>
        </tr>
    </table>
</asp:Content>

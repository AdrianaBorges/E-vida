<%@ Page Title="Configurações de IR" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="ConfiguracaoIr.aspx.cs" Inherits="eVidaIntranet.IR.ConfiguracaoIr" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <table width="100%">
        <tr>
            <td>
                <table width="90%" cellspacing="1px" cellpadding="10px" style="padding-left:75px" >
                    <tr>
                        <td colspan="5"><h1>HABILITAR MENU IR</h1></td>
                        <td colspan="3"><h1>Dia para geração do IR beneficiário</h1></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td style="width:100px">BENEFICIÁRIOS</td>
                        <td style="width:100px"><asp:DropDownList ID="dpdIrBeneficiario" runat="server">
                            <asp:ListItem Text="SIM" Value="S" />
                            <asp:ListItem Text="NÃO" Value="N" />
                            </asp:DropDownList></td>
                        <td style="width:100px">CREDENCIADOS</td>
                        <td style="width:100px"><asp:DropDownList ID="dpdIrCredenciados" runat="server">
                            <asp:ListItem Text="SIM" Value="S" />
                            <asp:ListItem Text="NÃO" Value="N" />
                            </asp:DropDownList></td>
                        <td>&nbsp;</td>
                        <td style="width:150px"><asp:DropDownList ID="dpdDiaIrBeneficiario" runat="server" /> de fevereiro</td>
                        <td>&nbsp;</td>
                    </tr>        
                    <tr>
                        <td colspan="8" align="center"> 
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                        </td>
                    </tr>

                 </table>
            </td>
        </tr>
        <tr>
            <td><hr /></td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td colspan="3"><h1>Anos disponíveis para beneficiários</h1></td>
                    </tr>
                    <tr>
                        <td><b>Incluir ano:</b></td>
                        <td><asp:TextBox ID="txtAno" runat="server" Width="100px" /></td>
                        <td><asp:Button ID="btnIncluirAno" runat="server" Text="Incluir" OnClick="btnIncluirAno_Click" /></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:GridView ID="gdvAnos" runat="server" AutoGenerateColumns="false" CssClass="tabela" Width="300px" OnRowCommand="gdvAnos_RowCommand">
                                <EmptyDataTemplate>
                                    <b>Não há anos configurados. Caso esteja disponível a opção, será apresentado apenas o ano anterior ao corrente.</b>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Ano" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# Container.DataItem %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton  ID="btnExcluir" runat="server" CommandName="CmdExcluir" CommandArgument='<%# ((GridViewRow)Container).DataItem %>' OnClientClick='<%# CreateJsFunctionGrid(Container, "openExcluir") %>'>
                                                <asp:Image ID="imgExcluir" runat="server" ImageUrl="~/img/remove.png" Height="25px" AlternateText="Excluir" ToolTip="Excluir" />
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>

    </table>
</asp:Content>

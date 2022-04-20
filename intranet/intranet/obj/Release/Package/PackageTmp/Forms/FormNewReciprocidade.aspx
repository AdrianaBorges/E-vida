<%@ Page Title="Autorização de Reciprocidade" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormNewReciprocidade.aspx.cs" Inherits="eVidaIntranet.Forms.FormNewReciprocidade" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    	function ConfigControlsCustom() {
        	//$("#tbPeriodo td").css({ "padding": "2px", "width": "100%" });
        }
        function goNova() {
        	window.location = "Reciprocidade.aspx";
        	return false;
        }
        function confirmSave() {
        	var msg = "Ao confirmar esta solicitação, o setor de cadastro providenciará a inclusão na reciprocidade e o informará através de e-mail as orientações cabíveis quando da providência. Só confirme caso tenha certeza. Não há necessidade de entregar o formulário na E-VIDA";
        	return confirm(msg);
        }

        function openEdit() {
        	window.location = "FormReciprocidade.aspx?id=" + $('#<%= litProtocolo.ClientID %>').val();
        	return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="placeHolder" runat="server">
    <div id="conteudo">
        <asp:HiddenField ID="litProtocolo" runat="server" />
        <h2>1 - Dados do Titular</h2>
	    <table id="tbTitular" border="1">
            <tr>
				<td rowspan="2" valign="top">
                    <asp:Label ID="lblCartao" runat="server" Text="Cartão" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtCartao" runat="server" Width="150px" OnTextChanged="txtCartao_TextChanged" AutoPostBack="true"/>
                </td>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>					
                </td>                
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:Label ID="txtEmailTitular" runat="server" Width="400px" />
                </td>
            </tr>
        </table>
        <br />
        
        <h2>2 - Período e Região para atendimento</h2>
	    <table id="tbPeriodo" border="1" class="tabelaForm" style="width:100%">
            <tr>
                <td colspan="2" style="width:100%"><b>PERÍODO DE PERMANÊNCIA NA LOCALIDADE:</b>&nbsp;
                    <asp:TextBox ID="txtInicio" runat="server" CssClass="calendario" MaxLength="10" Width="120px" /> &nbsp;A&nbsp;
                    <asp:TextBox ID="txtFim" runat="server" CssClass="calendario" MaxLength="10" Width="120px" />
                </td>
            </tr>
            <tr>
                <td style="width:120px"><b>UF</b><br />
                    <asp:DropDownList ID="dpdUf" runat="server" Width="100px" DataTextField="sigla" DataValueField="sigla" AutoPostBack="true" OnSelectedIndexChanged="dpdUf_SelectedIndexChanged" />
                </td>
                <td><b>MUNÍCIPIO</b><br />
                    <asp:DropDownList ID="dpdMunicipio" runat="server" Width="330px" DataTextField="NO_LOCALIDADE" DataValueField="NO_LOCALIDADE" />
                </td>
            </tr>
	    </table>
        
        <h2>3 - Beneficiários para Reciprocidade</h2>
        <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false" 
            OnRowDataBound="gdvDependentes_RowDataBound" OnRowCommand="gdvDependentes_RowCommand"
            DataKeyNames="cd_beneficiario, BI3_TIPO, BA3_DATBAS">
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:TemplateField HeaderText="Seq.">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNum" runat="server" />
                        <asp:ImageButton ID="lnkRemoverDependente" runat="server" ImageUrl="~/img/remove.png" CommandName="Excluir" CommandArgument='<%# Container.DataItemIndex  %>' AlternateText="Remover" ToolTip="Remover" /> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="BA1_NOMUSR" />
                <asp:BoundField HeaderText="Parentesco" DataField="BRP_DESCRI" />
                <asp:BoundField HeaderText="Titular/Dependente" DataField="BA1_TIPUSU" />                
                <asp:BoundField HeaderText="Data Nascimento" DataField="BA1_DATNAS" DataFormatString="{0:dd/MM/yyyy}" />                
                <asp:BoundField HeaderText="CPF" DataField="BA1_CPFUSR" />               
                <asp:BoundField HeaderText="Plano Vinculado" DataField="BI3_DESCRI" />
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnIncluirDep" runat="server" Text="Restaurar todos beneficiários" OnClick="btnIncluirDep_Click" />

        <h2>4 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Declaro estar ciente que a validade da autorização para utilização da reciprocidade será a considerada no período de permanência na localidade.</p>
            <p>Declaro que a solicitação de renovação da carteira / autorização para atendimento via reciprocidade é de minha responsabilidade como titular, onde se necessário for, solicitarei com antecedência mínima de 15 dias. </p>
            <p>Em caso de uso indevido, estou ciente que participarei integralmente nas despesas referentes à utilização dos planos a que estou vinculado.</p>
	    </div>
        
        <div>
            <br />
	        (Local e Data) <asp:TextBox ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <asp:Literal ID="ltData" runat="server" /> 
	        <br />            
	    </div>

        <div>
            <table width="100%">
                <tr>
                    <td align="center"><asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmSave()" />
						<asp:Button ID="btnEnviar" runat="server" Text="Autorizar formulário" OnClientClick="return openEdit()" Visible="false" />
                    </td>
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>

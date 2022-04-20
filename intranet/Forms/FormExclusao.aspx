<%@ Page Title="SOLICITAÇÃO DE EXCLUSÃO DE BENEFICIÁRIOS" Language="C#" MasterPageFile="~/Internal/Internal.Master" AutoEventWireup="true" CodeBehind="FormExclusao.aspx.cs" Inherits="eVidaIntranet.Forms.FormExclusao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script type="text/javascript">
    	function ConfigControlsCustom() {
    		$("#<%= tbSelecao.ClientID %> ").css({ "width": "100%" });
        	$("#<%= tbSelecao.ClientID %> td").css({ "padding": "2px" });
        }
        function goNova() {
            window.location = "FormExclusao.aspx";
            return false;
        }
        function confirmSave() {
        	return true;
        }
        function openPdf() {
        	var id = $('#<%= litProtocolo.ClientID%>').val();
        	openReport(RELATORIO_EXCLUSAO, "ID=" + id);
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
				    <asp:TextBox ID="txtCartao" runat="server" Width="150px" AutoPostBack="true" OnTextChanged="txtCartao_TextChanged"/>
                </td>
			    <td>
				    <asp:Label ID="lblNomeTitular" runat="server" Text="Nome Completo" Font-Bold="true" /><br />
				    <asp:Label ID="txtNomeTitular" runat="server" Width="400px"/>
                </td>
            </tr>
            <tr>
                <td>
				    <asp:Label ID="lblEmailTitular" runat="server" Text="E-mail" Font-Bold="true" /><br />
				    <asp:TextBox ID="txtEmailTitular" runat="server" Width="400px" MaxLength="150" />
                </td>
            </tr>
        </table>
        <br />
        
        <h2>2 - Beneficiários para Exclusão</h2>
	    <table id="tbSelecao" runat="server" border="1" class="tabelaForm">
            <tr>
                <td colspan="3"><asp:DropDownList ID="dpdDependente" runat="server" DataValueField="Key" DataTextField="Value" OnSelectedIndexChanged="dpdDependente_SelectedIndexChanged" AutoPostBack="true" Width="500px" /></td>
				<td rowspan="2" style="width:300px"><asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" /></td>
            </tr>
            <tr style="height:50px">
                <td><b>PARENTESCO</b><br />
                    <asp:Label ID="txtParentesco" runat="server" Width="250px"/>
                </td>
                <td><b>PLANO</b><br />
                    <asp:Label ID="txtPlano" runat="server" Width="250px" />
                </td>
                <td><b>IDADE</b><br />
                    <asp:Label ID="txtIdade" runat="server" Width="80px" />
                </td>
            </tr>            
	    </table>
        <br />
        <asp:GridView ID="gdvDependentes" runat="server" Width="100%" AutoGenerateColumns="false" 
            OnRowDataBound="gdvDependentes_RowDataBound" OnRowCommand="gdvDependentes_RowCommand"
            DataKeyNames="Cdusuario, CodPlano" CssClass="tabela">
            <AlternatingRowStyle CssClass="tbDependenteAlt" />
            <RowStyle CssClass="tbDependente" BorderWidth="1px" />
            
            <Columns>
                <asp:TemplateField HeaderText="Seq.">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNum" runat="server" />
                        <asp:ImageButton ID="lnkRemoverDependente" runat="server" ImageUrl="~/img/remove.png" CommandName="Excluir" CommandArgument='<%# Container.DataItemIndex  %>' AlternateText="Remover" ToolTip="Remover" /> 
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Nome Beneficiário" DataField="Nome" />
                <asp:BoundField HeaderText="Parentesco" DataField="Parentesco" />               
                <asp:BoundField HeaderText="Plano Vinculado" DataField="Plano" />
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnLimparDep" runat="server" Text="Limpar seleção" OnClick="btnLimparDep_Click" />

        <h2>3 - DECLARACAO</h2>
	    <div class="observacao">
		    <p>Compete ao beneficiário titular a devolução de sua carteira de identificação no momento do seu desligamento do plano, 
				bem como a devolução das carteiras de identificação dos seus dependentes vinculados aos planos de saúde quando perderem essa condição.</p>
            <p>Em caso de uso indevido e da não devolução das carteiras de identificação, declaro que estou ciente que participarei 
				integralmente nas despesas referentes à utilização do plano de saúde.
            </p>
			<p>O desligamento do titular ou de seus dependentes não exime o beneficiário titular de quitar eventuais débitos com a 
				E-VIDA, incluindo os valores de contribuição mensal e de co-participação.
			</p>
	    </div>

        <h2>4 - CONSEQUÊNCIAS DO CANCELAMENTO OU EXCLUSÃO DO CONTRATO DE PLANO DE SAÚDE</h2>
	    <div class="observacao">
            <p>Resolução Normativa nº 412, de 10 de novembro de 2016, da ANS.</p>
            <p>Art. 15. Recebida pela operadora ou administradora de benefícios, a solicitação do cancelamento do contrato de plano de saúde individual ou familiar ou de exclusão de beneficiários em plano coletivo empresarial ou coletivo por adesão, a operadora ou administradora de benefícios, destinatária do pedido, deverá prestar de forma clara e precisa, no mínimo, as seguintes informações:</p>
            <p>I – eventual ingresso em novo plano de saúde poderá importar:</p>
            <p>a) no cumprimento de novos períodos de carência, observado o disposto no inciso V do artigo 12, da Lei nº 9.656, de 3 de junho de 1998;</p>
            <p>b) na perda do direito à portabilidade de carências, caso não tenha sido este o motivo do pedido, nos termos previstos na RN nº 186, de 14 de janeiro de 2009, que dispõe, em especial, sobre a regulamentação da portabilidade das carências previstas no inciso V do art. 12 da Lei nº 9.656, de 3 de junho de 1998;</p>
            <p>c) no preenchimento de nova declaração de saúde, e, caso haja doença ou lesão preexistente – DLP, no cumprimento de Cobertura Parcial Temporária – CPT, que determina, por um período ininterrupto de até 24 meses, a partir da data da contratação ou adesão ao novo plano, a suspensão da cobertura de Procedimentos de Alta Complexidade (PAC), leitos de alta tecnologia e procedimentos cirúrgicos;</p>
            <p>d) na perda imediata do direito de remissão, quando houver, devendo o beneficiário arcar com o pagamento de um novo contrato de plano de saúde que venha a contratar;</p>
            <p>II - efeito imediato e caráter irrevogável da solicitação de cancelamento do contrato ou exclusão de beneficiário, a partir da ciência da operadora ou administradora de benefícios;</p>
            <p>III – as contraprestações pecuniárias vencidas e/ou eventuais coparticipações devidas, nos planos em pré-pagamento ou em pós-pagamento, pela utilização de serviços realizados antes da solicitação de cancelamento ou exclusão do plano de saúde são de responsabilidade do beneficiário;</p>
            <p>IV - as despesas decorrentes de eventuais utilizações dos serviços pelos beneficiários após a data de solicitação de cancelamento ou exclusão do plano de saúde, inclusive nos casos de urgência ou emergência, correrão por sua conta;</p>
            <p>V – a exclusão do beneficiário titular do contrato individual ou familiar não extingue o contrato, sendo assegurado aos dependentes já inscritos o direito à manutenção das mesmas condições contratuais, com a assunção das obrigações decorrentes; e</p>
            <p>VI – a exclusão do beneficiário titular do contrato coletivo empresarial ou por adesão observará as disposições contratuais quanto à exclusão ou não dos dependentes, conforme o disposto no inciso II do parágrafo único do artigo 18, da RN nº 195, de 14 de julho de 2009, que dispõe sobre a classificação e características dos planos privados de assistência à saúde, regulamenta a sua contratação, institui a orientação para contratação de planos privados de assistência à saúde e dá outras providências.</p>
	    </div>
          
	    <div>
            <b>Observações: </b> (incluir link da chamada J4Call) <br />
		    <asp:TextBox ID="txtObs" runat="server" Width="90%" TextMode="MultiLine" Rows="3" />
	    </div>

        <div>
            <br />
            Declaro, para os devidos fins, que tenho plena ciência das consequências do cancelamento ou exclusão do contrato de plano de saúde.
            <br />
        </div>

        <div>
            <br />
	        (Local e Data) <asp:TextBox ID="txtLocal" runat="server" Width="150px" CssClass="inputInline" />, <asp:Literal ID="ltData" runat="server" /> 
	        <br />            
	    </div>
		<div style="text-align: center">
			<br /><br /><br /><br />
			____________________________________________________________<br />
			ASSINATURA DO TITULAR
		</div>
        <div>
            <table width="100%">
                <tr>
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" OnClientClick="return confirmSave()" />
					<asp:ImageButton ID="btnPdf" runat="server" Text="Imprimir" OnClientClick="return openPdf()" Visible="false" ImageUrl="~/img/PDF.png" Height="30px" />
                    <asp:Button ID="btnNova" runat="server" Text="Nova Solicitação" OnClientClick="return goNova()" Visible="false" />
                </tr>
            </table>
	    
        
        </div>
    </div>
</asp:Content>

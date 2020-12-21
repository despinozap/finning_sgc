<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Subareas.aspx.cs" Inherits="NCCSAN.Administracion.Subareas" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Popup_background
        {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }
        
        .Popup_frontground
        {
            background-color: White;
            padding: 30px; /*border-style: dotted;
            border-width: 2px;*/
            box-shadow: 10px 10px 5px #555555;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Lista de Sub-áreas</h1>
    </div>
    <asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnSubareas" runat="server" HorizontalAlign="Center">
                <table class="tableControls" align="center" width="50%">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Sub-áreas (Área:
                                <asp:Label ID="lbNombreArea" runat="server" Text=""></asp:Label>)</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:GridView ID="gvSubareas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                HorizontalAlign="Center" Width="100%" ShowFooter="True" ShowHeaderWhenEmpty="True"
                                DataSourceID="SDSSubareas" OnRowCommand="gvSubareas_RowCommand">
                                <AlternatingRowStyle CssClass="rowDataAlt" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Nombre sub-área">
                                        <ItemTemplate>
                                            <asp:Label ID="lbNombre" runat="server" Text='<%#Eval("nombre_subarea") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <FooterTemplate>
                                            <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="AgregarSubarea" />
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:ButtonField HeaderText="Editar" ButtonType="Image" ImageUrl="~/Images/Icon/edit.png"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" Text="Editar"
                                        CommandName="EditarSubarea">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                    <asp:ButtonField ButtonType="Image" HeaderText="Eliminar" ImageUrl="~/Images/Icon/remove_file.png"
                                        Text="Eliminar" CommandName="EliminarSubarea">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:ButtonField>
                                </Columns>
                                <HeaderStyle BackColor="#666666" ForeColor="White" />
                                <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                    PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                    NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                <PagerStyle HorizontalAlign="Center" />
                                <RowStyle CssClass="rowData" />
                                <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                <EmptyDataTemplate>
                                    <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                    <asp:Label ID="lbEmptyList" runat="server" Text="  No se han encontrado registros"
                                        Font-Size="14px"></asp:Label>
                                    <br />
                                    <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="AgregarSubarea" />
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:SqlDataSource ID="SDSSubareas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                SelectCommand="SELECT nombre_subarea FROM CentroAreaSubarea WHERE (id_centro=@id_centro) AND (nombre_area=@nombre_area)">
                                <SelectParameters>
                                    <asp:SessionParameter Name="id_centro" SessionField="id_centro" />
                                    <asp:SessionParameter Name="nombre_area" SessionField="nombre_area" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                </table>
                <br />
                <table class="tableControls" align="center" width="50%">
                    <tr>
                        <td class="tdTitle">
                            <h1>
                                Supervisores</h1>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:Label ID="Label1" runat="server" Text="Sub-área"></asp:Label>
                            <asp:DropDownList ID="ddlSubareas" runat="server" DataSourceID="SDSSubareas" DataTextField="nombre_subarea"
                                DataValueField="nombre_subarea" OnDataBound="ddlSubareas_DataBound" Width="190px"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlSubareas_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdUnique">
                            <asp:UpdatePanel ID="upSupervisores" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="gvSupervisores" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center"
                                        ShowFooter="True" Width="690px" ShowHeaderWhenEmpty="True" OnRowCommand="gvSupervisores_RowCommand">
                                        <Columns>
                                            <asp:TemplateField HeaderText="RUT">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbRUT" runat="server" Text=""></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Nombre">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbNombre" runat="server" Text=""></asp:Label>
                                                </ItemTemplate>
                                                <FooterStyle HorizontalAlign="Center" />
                                                <FooterTemplate>
                                                    <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="BuscarSupervisor" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Centro">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbNombreCentro" runat="server" Text=""></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cargo">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbCargo" runat="server" Text=""></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Antigüedad">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbAntiguedad" runat="server" Text=""></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:ButtonField CommandName="DelSupervisor" HeaderText="Eliminar" ButtonType="Image"
                                                ImageUrl="~/Images/Icon/remove_user.png">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonField>
                                        </Columns>
                                        <HeaderStyle BackColor="#666666" ForeColor="White" />
                                        <EmptyDataRowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <EmptyDataTemplate>
                                            <asp:Image ID="imgEmptyList" runat="server" ImageUrl="~/Images/empty.png" ImageAlign="Middle" />
                                            <asp:Label ID="lbEmptyList" runat="server" Text="  No se han cargado personas" Font-Size="14px"></asp:Label>
                                            <br />
                                            <asp:Button ID="btAdd" runat="server" Text="+ Agregar" CommandName="BuscarSupervisor" />
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <table align="center">
                    <tr>
                        <td>
                            <asp:ImageButton ID="ibVolver" runat="server" ImageUrl="~/Images/Button/back.png"
                                ImageAlign="AbsMiddle" CssClass="submitPanelButton" OnClick="ibVolver_Click" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hfAgregarSubarea" runat="server" />
                <asp:ModalPopupExtender ID="hfAgregarSubarea_ModalPopupExtender" runat="server" DynamicServicePath=""
                    Enabled="True" TargetControlID="hfAgregarSubarea" PopupControlID="pnAgregarSubarea"
                    BackgroundCssClass="Popup_background">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upAgregarSubarea" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnAgregarSubarea" runat="server" CssClass="Popup_frontground" HorizontalAlign="Center">
                            <table class="tableControls" align="center">
                                <tr>
                                    <td colspan="2" class="tdTitle">
                                        <h1>
                                            Agregar Sub-área
                                        </h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:Label ID="Label5" runat="server" Text="Nombre"></asp:Label>
                                        &nbsp;<asp:Label ID="Label7" runat="server" Text="*" ForeColor="Red" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td class="tdUnique">
                                        <asp:TextBox ID="txtNombreSubarea" runat="server" MaxLength="70" Width="220px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btRegistrarSubarea" runat="server" Text="Registrar" OnClick="btRegistrarSubarea_Click"
                                            CssClass="submitButton" />
                                        <asp:Button ID="btCancelarSubarea" runat="server" Text="Cancelar" OnClick="btCancelarSubarea_Click"
                                            CssClass="submitButton" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Confirm Eliminar Subarea -->
                <asp:HiddenField ID="hfEliminarSubarea" runat="server" />
                <asp:ModalPopupExtender ID="hfEliminarSubarea_ModalPopupExtender" runat="server"
                    BackgroundCssClass="Popup_background" DynamicServicePath="" Enabled="True" PopupControlID="upEliminarSubarea"
                    TargetControlID="hfEliminarSubarea">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upEliminarSubarea" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnEliminarSubarea" runat="server" CssClass="Popup_frontground">
                            <table align="center">
                                <tr>
                                    <td>
                                        <asp:Label ID="lbMessageEliminarSubarea" runat="server" CssClass="messageBoxMessage"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btEliminarSubarea" runat="server" Text="Aceptar" CssClass="submitButton"
                                            OnClick="btEliminarArea_Click" />
                                        <asp:Button ID="btCancelarEliminarSubarea" runat="server" Text="Cancelar" CssClass="submitButton"
                                            OnClick="btCancelarEliminarSubarea_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Confirm Eliminar Subarea !-->
                <!-- Panel Buscar Persona -->
                <asp:HiddenField ID="hfBuscarPersona" runat="server" />
                <asp:ModalPopupExtender ID="hfBuscarPersona_ModalPopupExtender" runat="server" BackgroundCssClass="Popup_background"
                    DynamicServicePath="" Enabled="True" PopupControlID="upBuscarPersona" TargetControlID="hfBuscarPersona">
                </asp:ModalPopupExtender>
                <asp:UpdatePanel ID="upBuscarPersona" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnBuscarPersona" runat="server" CssClass="Popup_frontground">
                            <table class="tableControls">
                                <tr>
                                    <td colspan="2" class="tdTitle">
                                        <h1>
                                            Buscar persona</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:Label ID="Label33" runat="server" Text="Apellido"></asp:Label>
                                        <asp:TextBox ID="txtBuscarPersonaApellido" runat="server" AutoPostBack="True"></asp:TextBox>
                                        <asp:Button ID="btBuscarPersonaBuscar" runat="server" Text="Buscar" />
                                        <asp:Button ID="btBuscarPersonaLimpiar" runat="server" Text="Limpiar" OnClick="btBuscarPersonaLimpiar_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="tdUnique">
                                        <asp:GridView ID="gvBuscarPersonas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                            DataKeyNames="rut" DataSourceID="SDSBuscarPersonas" HorizontalAlign="Center"
                                            OnRowCommand="gvBuscarPersonas_RowCommand" Width="1010px">
                                            <AlternatingRowStyle BackColor="#EDEDED" />
                                            <Columns>
                                                <asp:ButtonField HeaderText="Agregar" ButtonType="Image" ImageUrl="~/Images/Icon/add_user.png"
                                                    CommandName="AddSupervisor" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:ButtonField>
                                                <asp:BoundField DataField="rut" HeaderText="RUT" ReadOnly="True" SortExpression="rut" />
                                                <asp:BoundField DataField="nombre" HeaderText="Nombre" SortExpression="nombre" />
                                                <asp:BoundField DataField="edad" HeaderText="Edad" ReadOnly="True" SortExpression="edad">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="nombre_centro" HeaderText="Centro" SortExpression="nombre_centro">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="cargo" HeaderText="Cargo" SortExpression="cargo">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="antiguedad" HeaderText="Antigüedad" ReadOnly="True" SortExpression="antiguedad">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle BackColor="#666666" ForeColor="White" />
                                            <PagerSettings FirstPageText="" LastPageText="" Mode="NextPreviousFirstLast" NextPageText=""
                                                PreviousPageText="" FirstPageImageUrl="~/Images/Icon/first.png" LastPageImageUrl="~/Images/Icon/last.png"
                                                NextPageImageUrl="~/Images/Icon/next.png" PreviousPageImageUrl="~/Images/Icon/previous.png" />
                                            <PagerStyle HorizontalAlign="Center" />
                                        </asp:GridView>
                                        <asp:SqlDataSource ID="SDSBuscarPersonas" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
                                            SelectCommand="SELECT p.rut, p.nombre, DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, c.nombre AS nombre_centro, p.cargo, DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad FROM Persona p, Centro c WHERE (p.fecha_retiro IS NULL) AND (p.nombre_clasificacionpersona&lt;&gt;'Clientes') AND (p.nombre_clasificacionpersona&lt;&gt;'RSP') AND (p.nombre LIKE '%' + @nombre + '%') AND (p.id_centro=c.id) ORDER BY p.nombre ASC"
                                            OnSelected="SDSBuscarPersonas_Selected">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="txtBuscarPersonaApellido" Name="nombre" PropertyName="Text" />
                                            </SelectParameters>
                                        </asp:SqlDataSource>
                                        <div align="left" style="text-align: center">
                                            <asp:Literal ID="ltBuscarPersonaSummary" runat="server"></asp:Literal>
                                        </div>
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btBuscarPersonaCerrar" runat="server" Text="Cerrar" OnClick="btBuscarPersonaCerrar_Click"
                                            CssClass="submitButton" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Panel Buscar Persona !-->
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

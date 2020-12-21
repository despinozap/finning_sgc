<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DetallePersona.aspx.cs" Inherits="NCCSAN.Personas.DetallePersona" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="divTituloForm">
        <h1>
            Detalle de Persona</h1>
    </div>
    <asp:HiddenField ID="hfRutPersona" runat="server" />
    <asp:HiddenField ID="hfPreviousPage" runat="server" />
    <br />
    <asp:FormView ID="fvPersona" runat="server" DataKeyNames="rut" DataSourceID="SDSDetallePersona"
        HorizontalAlign="Center" Width="671px">
        <ItemTemplate>
            <table class="tableControls" align="center">
                <tr>
                    <td colspan="3" class="tdTitle">
                        <h1>
                            Información de la Persona
                        </h1>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label1" runat="server" Text="RUT" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label2" runat="server" Text="Nombre" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label3" runat="server" Text="Edad (años)" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast">
                        <asp:Label ID="lbRUT" runat="server" Text='<%# Eval("rut") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbNombre" runat="server" Text='<%# Eval("nombre") %>'></asp:Label>
                    </td>
                    <td class="tdLast" align="right">
                        <asp:Label ID="lbEdad" runat="server" Text='<%# Eval("edad") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label6" runat="server" Text="Email" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label4" runat="server" Text="ID Empleado" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label5" runat="server" Text="Centro" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast">
                        <asp:Label ID="lbEmail" runat="server" Text='<%# Eval("email") %>'></asp:Label>
                        <a href="mailto:<%# Eval("email") %>">
                            <asp:Image ID="imMailto" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/Icon/send_email.png" ToolTip="Enviar correo electrónico" />
                        </a>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbIDEmpleado" runat="server" Text='<%# Eval("id_empleado") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbCentro" runat="server" Text='<%# Eval("nombre_centro") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label7" runat="server" Text="Store" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label8" runat="server" Text="ID Store" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label9" runat="server" Text="Lob" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast">
                        <asp:Label ID="lbNombreStore" runat="server" Text='<%# Eval("nombre_store") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbIDStore" runat="server" Text='<%# Eval("id_store") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbLob" runat="server" Text='<%# Eval("lob") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label10" runat="server" Text="Cargo" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label11" runat="server" Text="Clasificación" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label12" runat="server" Text="Antigüedad (años)" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast">
                        <asp:Label ID="lbCargo" runat="server" Text='<%# Eval("cargo") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbFechaIngreso" runat="server" Text='<%# Eval("nombre_clasificacionpersona") %>'></asp:Label>
                    </td>
                    <td class="tdLast" align="right">
                        <asp:Label ID="lbAntiguedad" runat="server" Text='<%# Eval("antiguedad") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label17" runat="server" Text="Fecha de Ingreso" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label18" runat="server" Text="Fecha de Retiro" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label19" runat="server" Text="" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast" align="center">
                        <asp:Label ID="Label20" runat="server" Text='<%#  Eval("fecha_ingreso")  %>'></asp:Label>
                    </td>
                    <td class="tdLast" align="center">
                        <asp:Label ID="Label21" runat="server" Text='<%#Eval("fecha_retiro")%>'></asp:Label>
                    </td>
                    <td class="tdLast" align="right">
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:FormView>
    <asp:SqlDataSource ID="SDSDetallePersona" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
        SelectCommand="SELECT 
		p.rut, 
		p.nombre,
		DATEDIFF(YEAR, p.fecha_nacimiento, SYSDATETIME()) AS edad, 
		p.email, 
		CASE
			WHEN (p.id_empleado IS NOT NULL) THEN p.id_empleado
			ELSE '--'
			END
		AS id_empleado, 
		c.nombre AS nombre_centro, 
		CASE
			WHEN (p.id_store IS NOT NULL) THEN (SELECT s.nombre FROM Store s WHERE s.id=p.id_store)
			ELSE '--'
			END
		AS nombre_store,
		CASE
			WHEN (p.id_store IS NOT NULL) THEN p.id_store
			ELSE '--'
			END
		AS id_store,
		CASE
			WHEN (p.lob IS NOT NULL) THEN p.lob
			ELSE '--'
			END
		AS lob, 
		CASE
			WHEN (p.cargo IS NOT NULL) THEN p.cargo
			ELSE '--'
			END
		AS cargo, 
		p.nombre_clasificacionpersona, 
		CONVERT(VARCHAR(10), p.fecha_ingreso, 105) AS fecha_ingreso,
		CASE
			WHEN (p.fecha_retiro IS NOT NULL) THEN CONVERT(VARCHAR(10), p.fecha_retiro, 105)
			ELSE '--'
			END
		AS fecha_retiro,
		DATEDIFF(YEAR, p.fecha_ingreso, SYSDATETIME()) AS antiguedad 
FROM 
		Persona p, 
		Centro c
WHERE 
		(p.rut=@rut) AND 
		(p.id_centro=c.id)">
        <SelectParameters>
            <asp:ControlParameter ControlID="hfRutPersona" Name="rut" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:FormView ID="fvSupervisor" runat="server" HorizontalAlign="Center" Width="671px"
        DataSourceID="SDSDetalleSupervisor">
        <ItemTemplate>
            <table class="tableControls" align="center">
                <tr>
                    <td colspan="3">
                        <h1>
                        </h1>
                        <h1>
                        </h1>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" class="tdTitle">
                        <h1>
                            Información del Supervisor
                        </h1>
                    </td>
                </tr>
                <tr>
                    <td class="tdFirst">
                        <asp:Label ID="Label13" runat="server" Text="RUT" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label15" runat="server" Text="Nombre" Font-Bold="True"></asp:Label>
                    </td>
                    <td class="tdFirst">
                        <asp:Label ID="Label14" runat="server" Text="Antigüedad(años)" Font-Bold="True"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdLast">
                        <asp:Label ID="lbRUTSupervisor" runat="server" Text='<%# Eval("rut_supervisor") %>'></asp:Label>
                    </td>
                    <td class="tdLast">
                        <asp:Label ID="lbNombreSupervisor" runat="server" Text='<%# Eval("nombre_supervisor") %>'></asp:Label>
                    </td>
                    <td class="tdLast" align="right">
                        <asp:Label ID="Label16" runat="server" Text='<%# Eval("antiguedad_supervisor") %>'></asp:Label>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:FormView>
    <asp:SqlDataSource ID="SDSDetalleSupervisor" runat="server" ConnectionString="<%$ ConnectionStrings:NCCSANConnectionString %>"
        SelectCommand="SELECT s.rut AS rut_supervisor, s.nombre AS nombre_supervisor, DATEDIFF(YEAR, s.fecha_ingreso, SYSDATETIME()) AS antiguedad_supervisor FROM Persona s, Persona p WHERE (p.rut=@rut) AND (p.rut_supervisor=s.rut)">
        <SelectParameters>
            <asp:ControlParameter ControlID="hfRutPersona" Name="rut" PropertyName="Value" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br />
    <br />
    <table align="center">
        <tr>
            <td>
                <asp:ImageButton ID="ibVolver" runat="server" ImageUrl="~/Images/Button/back.png"
                    ImageAlign="AbsMiddle" OnClick="ibVolver_Click" CssClass="submitPanelButton" />
            </td>
        </tr>
    </table>
</asp:Content>

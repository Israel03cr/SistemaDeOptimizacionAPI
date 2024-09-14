public class ControlActividadDto
{
    public int ControlID { get; set; }  // Esto solo se utiliza en PUT y GET.
    public int ReservaID { get; set; }  // Solo se envía esto en POST.
    public int ActividadID { get; set; }  // Solo se envía esto en POST.
    public DateTime FechaActividad { get; set; }  // Solo se envía esto en POST.

    // Estos campos solo son para respuestas (GET) y no deben ser requeridos en POST ni PUT.
    public string NombrePerro { get; set; }
    public string NombreDueño { get; set; }
    public string NombreActividad { get; set; }
}

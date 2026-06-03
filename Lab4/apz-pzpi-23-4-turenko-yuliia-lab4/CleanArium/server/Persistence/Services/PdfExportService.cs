using Application.Abstractions;
using Domain.Models;

namespace Persistence.Services;

public class PdfExportService : IPdfExportService
{
    public byte[] ExportAquariums(IEnumerable<Aquarium> data)
    {
        using var ms = new MemoryStream();
        var doc = new iTextSharp.text.Document();
        var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, ms);

        doc.Open();
        doc.Add(new iTextSharp.text.Paragraph("Aquariums Export"));
        doc.Add(new iTextSharp.text.Paragraph(" "));

        foreach (var a in data)
        {
            doc.Add(new iTextSharp.text.Paragraph($"Aquarium: {a.Name}"));
            doc.Add(new iTextSharp.text.Paragraph($"Location: {a.Location}"));
            doc.Add(new iTextSharp.text.Paragraph($"Created: {a.CreatedAt}"));
            doc.Add(new iTextSharp.text.Paragraph("Devices:"));

            foreach (var d in a.Devices)
            {
                doc.Add(new iTextSharp.text.Paragraph($" - {d.DeviceType} [{d.DeviceStatus}]"));
            }

            doc.Add(new iTextSharp.text.Paragraph(" "));
        }

        doc.Close();
        return ms.ToArray();
    }
}
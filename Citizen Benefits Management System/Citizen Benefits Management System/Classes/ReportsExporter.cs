using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Citizen_Benefits_Management_System.Model;

namespace CitizenBenefitsManagementSystem.Classes
{
    public class ReportsExporter
    {
        /// <summary>
        /// Экспорт списка записей журнала событий в Excel.
        /// </summary>
        /// <param name="events">Список EventLog моделей</param>
        /// <param name="filePath">Путь для сохранения .xlsx</param>
        public void ExportEventLogToExcel(IEnumerable<EventLog> events, string filePath)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("EventLog");

            // Заголовки
            ws.Cell(1, 1).Value = "LogID";
            ws.Cell(1, 2).Value = "UserID";
            ws.Cell(1, 3).Value = "EventType";
            ws.Cell(1, 4).Value = "EventDescription";
            ws.Cell(1, 5).Value = "EntityType";
            ws.Cell(1, 6).Value = "EntityID";
            ws.Cell(1, 7).Value = "IPAddress";
            ws.Cell(1, 8).Value = "UserAgent";
            ws.Cell(1, 9).Value = "CreatedAt";

            int row = 2;
            foreach (var e in events)
            {
                ws.Cell(row, 1).Value = e.LogID;
                ws.Cell(row, 2).Value = e.UserID;
                ws.Cell(row, 3).Value = e.EventType;
                ws.Cell(row, 4).Value = e.EventDescription;
                ws.Cell(row, 5).Value = e.EntityType;
                ws.Cell(row, 6).Value = e.EntityID;
                ws.Cell(row, 7).Value = e.IPAddress;
                ws.Cell(row, 8).Value = e.UserAgent;
                ws.Cell(row, 9).Value = e.CreatedAt;
                row++;
            }

            // Форматирование столбцов
            ws.Columns().AdjustToContents();

            // Убедиться, что папка существует
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            workbook.SaveAs(filePath);
        }

        /// <summary>
        /// Экспорт отчёта по льготным категориям в Word (таблица).
        /// </summary>
        /// <param name="rows">Список строк для отчёта (реализуй DTO или используй Citizen + Benefit)</param>
        /// <param name="filePath">Путь для сохранения .docx</param>
        public void ExportBenefitsReportToWord(IEnumerable<BenefitReportRow> rows, string filePath)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            var doc = DocX.Create(filePath);
            doc.InsertParagraph("Отчёт по льготным категориям").FontSize(16).Bold();

            // Подготовим таблицу
            var list = new List<BenefitReportRow>(rows);
            var table = doc.AddTable(list.Count + 1, 7); // пример: 7 колонок
            table.Design = TableDesign.LightListAccent1;

            // Заголовки
            table.Rows[0].Cells[0].Paragraphs[0].Append("№");
            table.Rows[0].Cells[1].Paragraphs[0].Append("ФИО");
            table.Rows[0].Cells[2].Paragraphs[0].Append("Дата рождения");
            table.Rows[0].Cells[3].Paragraphs[0].Append("Адрес");
            table.Rows[0].Cells[4].Paragraphs[0].Append("Категория льготы");
            table.Rows[0].Cells[5].Paragraphs[0].Append("Основание");
            table.Rows[0].Cells[6].Paragraphs[0].Append("Примечание");

            for (int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                table.Rows[i + 1].Cells[0].Paragraphs[0].Append((i + 1).ToString());
                table.Rows[i + 1].Cells[1].Paragraphs[0].Append(r.FullName);
                table.Rows[i + 1].Cells[2].Paragraphs[0].Append(r.BirthDate?.ToString("yyyy-MM-dd") ?? "");
                table.Rows[i + 1].Cells[3].Paragraphs[0].Append(r.Address ?? "");
                table.Rows[i + 1].Cells[4].Paragraphs[0].Append(r.CategoryName ?? "");
                table.Rows[i + 1].Cells[5].Paragraphs[0].Append(r.LegalBasis ?? "");
                table.Rows[i + 1].Cells[6].Paragraphs[0].Append(r.Notes ?? "");
            }

            doc.InsertTable(table);
            doc.Save();
        }
    }

    // DTO для примера
    public class BenefitReportRow
    {
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string CategoryName { get; set; }
        public string LegalBasis { get; set; }
        public string Notes { get; set; }
    }
}

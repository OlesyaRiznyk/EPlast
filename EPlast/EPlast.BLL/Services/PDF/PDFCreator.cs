﻿using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;
using System.Text;

namespace EPlast.BLL
{
    internal class PdfCreator : IPdfCreator
    {
        private readonly IPdfDocument document;
        private readonly PdfSharp.Pdf.PdfDocument document;

        public PdfCreator(IPdfDocument document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public byte[] GetPDFBytes()
        {
            CreatePDF();
            byte[] fileContents;
            using (var stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, true);
                fileContents = stream.ToArray();
            }

            return fileContents;
        }

        private void CreatePDF()
        {
            renderer = new PdfDocumentRenderer(true)
            {
                Document = document.GetDocument()
            };

            renderer.RenderDocument();
        }
    }
}
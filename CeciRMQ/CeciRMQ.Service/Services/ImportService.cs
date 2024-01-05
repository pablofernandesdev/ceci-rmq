using AutoMapper;
using CeciRMQ.Domain.DTO.Commons;
using CeciRMQ.Domain.DTO.Email;
using CeciRMQ.Domain.DTO.Import;
using CeciRMQ.Domain.DTO.User;
using CeciRMQ.Domain.Entities;
using CeciRMQ.Domain.Interfaces.Repository;
using CeciRMQ.Domain.Interfaces.Service;
using CeciRMQ.Infra.CrossCutting.Extensions;
using CeciRMQ.Infra.CrossCutting.Helper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CeciRMQ.Service.Services
{
    public class ImportService : IImportService
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMessageService<EmailRequestDTO> _messageService;

        public ImportService(IEmailService emailService,
            IUnitOfWork uow,
            IMapper mapper,
            IMessageService<EmailRequestDTO> messageService)
        {
            _emailService = emailService;
            _uow = uow;
            _mapper = mapper;
            _messageService = messageService;
        }

        public async Task<ResultResponse> ImportUsersAsync(FileUploadDTO model)
        {
            var response = new ResultResponse();

            try
            {
                var fileName = Path.GetFileName(Guid.NewGuid().ToString() + model.File.FileName);
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\UploadFiles", fileName);
                var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net6.0", string.Empty)), 
                    @"wwwroot\UploadFiles", 
                    fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                if (Path.GetExtension(filePath).Equals(".csv"))
                {
                    filePath = ConvertWithClosedXml(filePath);
                }

                var users = new List<UserImportDTO>();

                var basicRole = await _uow.Role.GetBasicProfile();

                using (var excelWorkbook = new XLWorkbook(filePath))
                {
                    var nonEmptyDataRows = excelWorkbook.Worksheets.FirstOrDefault().RowsUsed().Skip(1);

                    foreach (var dataRow in nonEmptyDataRows)
                    {
                        var cellName = dataRow.Cell(1).Value;
                        var cellEmail = dataRow.Cell(2).Value;

                        var password = PasswordExtension.GeneratePassword(2, 2, 2, 2);

                        users.Add(new UserImportDTO
                        {
                            Name = cellName.ToString(),
                            Email = cellEmail.ToString(),
                            RoleId = basicRole.Id,
                            Password = PasswordExtension.EncryptPassword(StringHelper.Base64Encode(password)),
                            PasswordBase64Decode = password
                        });
                    }
                }

                await _uow.User.AddRangeAsync(_mapper.Map<IEnumerable<User>>(users));
                await _uow.CommitAsync();

                //rabbit
                foreach (var item in users)
                {
                    _messageService.AddQueueItem(new EmailRequestDTO
                    {
                        Body = "Your registration was carried out in the application. Use the password <b>" + item.PasswordBase64Decode + "</b> on your first access to the application.",
                        Subject = item.Name,
                        ToEmail = item.Email
                    });
                }

                File.Delete(filePath);

                response.Message = "Users imported successfully.";
            }
            catch (Exception ex)
            {
                response.Message = "Unable to import users.";
                response.Exception = ex;
            }

            return response;
        }

        private static string ConvertWithClosedXml(string atualFile)
        {
            //var newFile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\UploadFiles", Guid.NewGuid().ToString() + ".xlsx");
            var newFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net6.0", string.Empty)),
                @"wwwroot\UploadFiles",
                Guid.NewGuid().ToString() + ".xlsx");

            var csvLines = File.ReadAllLines(atualFile, Encoding.UTF8).Select(a => a.Split(';'));

            int rowCount = 0;
            int colCount = 0;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add();

                rowCount = 1;

                foreach (var line in csvLines)
                {
                    colCount = 1;
                    foreach (var col in line)
                    {
                        worksheet.Cell(rowCount, colCount).Value = col;
                        colCount++;
                    }
                    rowCount++;
                }

                File.Delete(atualFile);

                workbook.SaveAs(newFile);
            }

            return newFile;
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using secondVersionFlowSync.Data;
using secondVersionFlowSync.DTOs;
using secondVersionFlowSync.DTOs.Auth;
using secondVersionFlowSync.Models;
using secondVersionFlowSync.services.EmailService;
using System;
using System.Web;
using Task = System.Threading.Tasks.Task;

namespace secondVersionFlowSync.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly ApplicationDbContext context;

        public AccountController(UserManager<AppUser> userManager, IEmailService emailService, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.context = context;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // تحقق مما إذا كان اسم المستخدم موجودًا مسبقًا
            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                throw new Exception("Email is already taken.");

            }

            // تحقق مما إذا كان اسم المستخدم الموجود في البريد الإلكتروني موجودًا مسبقًا
            var existingUserByUsername = await userManager.FindByNameAsync(model.Email.Split('@')[0]);
            if (existingUserByUsername != null)
            {
                throw new Exception("Username is already taken.");
            }

            // تحقق من وجود قائد فريق (TeamLeader) في النظام
            if (model.Role == Role.Member && !userManager.Users.Any(u => u.Role == Role.Leader))
            {
                throw new Exception("لا يمكن للميمبر التسجيل بدون وجود ليدر.");
            }

            var user = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Role = model.Role,
                UserName = model.Email,
            };

            // تحقق من وجود قائد فريق (TeamLeader) في النظام
            if (model.Role == Role.Member && !userManager.Users.Any(u => u.Role == Role.Leader))
            {
                throw new Exception("لا يمكن للميمبر التسجيل بدون وجود ليدر.");
            }


            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                throw new Exception("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            }
            // إذا كان الدور TeamLeader، نرسل له بريد التأكيد
            if (user.Role == Role.Leader)
            {
                var emailDto = new EmailDto
                {
                    To = user.Email,
                    Subject = "تحقق من حسابك كـ Team Leader",
                    Body = $"مرحباً، تم تسجيلك كـ Team Leader. يرجى التحقق من بريدك الإلكتروني باستخدام الرابط التالي: {GenerateEmailConfirmationLink(user)}",
                };
                await emailService.sendEmailAsync(emailDto);
            }
            else if (user.Role == Role.Member)
            {
                // إرسال طلب إلى القائد للموافقة على العضو
                var leader = userManager.Users.FirstOrDefault(u => u.Role == Role.Leader);
                if (leader != null)
                {
                    var pendingRequest = new PendingMemberRequest
                    {
                        MemberId = user.Id,
                        LeaderId = leader.Id
                    };
                    await context.PendingMemberRequests.AddAsync(pendingRequest);
                    await context.SaveChangesAsync();
                }
            }

            return Ok(new { message = "success" });


        }

        // موافقة القائد على العضو
        [HttpPost("approve-member/{requestId}")]
        public async Task<IActionResult> ApproveMember(int requestId)
        {
            var pendingRequest = await context.PendingMemberRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);
            if (pendingRequest == null)
            {
                throw new Exception("طلب العضوية غير موجود.");
            }

            // التأكد من أن القائد هو من يوافق
            var currentUser = await userManager.GetUserAsync(User);  // المستخدم الحالي (القائد)
            if (pendingRequest.LeaderId != currentUser.Id)
            {
                throw new Exception("أنت لست القائد المعني.");
            }

            // الموافقة على الطلب
            pendingRequest.IsApproved = true;
            await context.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات

            // إرسال بريد إلكتروني للميمبر بعد الموافقة
            var member = await userManager.FindByIdAsync(pendingRequest.MemberId);
            if (member != null)
            {
                var emailDto = new EmailDto
                {
                    To = member.Email,
                    Subject = "تمت الموافقة على طلبك كـ Team Member",
                    Body = $"مرحباً، تم قبولك كـ Team Member. يرجى التحقق من بريدك الإلكتروني باستخدام الرابط التالي: {GenerateEmailConfirmationLink(member)}",
                };
                await emailService.sendEmailAsync(emailDto);
            }

            return Ok("تمت الموافقة على العضو بنجاح.");
        }

        // إنشاء رابط تأكيد البريد الإلكتروني
        private string GenerateEmailConfirmationLink(AppUser user)
        {
            var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            var confirmationLink = Url.Action("ConfirmEmail", "User", new { userId = user.Id, token = token }, Request.Scheme);
            return confirmationLink;
        }

        // تأكيد البريد الإلكتروني
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                throw new Exception("معرف المستخدم أو الرمز غير صالح.");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("المستخدم غير موجود.");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("تم تأكيد البريد الإلكتروني بنجاح.");
            }

            throw new Exception("فشل تأكيد البريد الإلكتروني.");
        }
    }


}









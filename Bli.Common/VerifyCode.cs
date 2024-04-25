using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Common
{
    public class VerifyCode
    {
        public static readonly string Error_Code = "验证码好像错了，再检查一下叭";
        public static readonly string UserName_Exist = "用户名视乎重复了";
        public static readonly string Account_Locked = "哦哦，用户被锁定请稍后重试吧";

        public static readonly string Login_Fail = "登录失败了，再检查一下喵";

        public static readonly string Auto_RegisterByEmial = "已为发送邮件了喵，查看邮件激活账号";
        public static readonly string Auto_RegisterByPhone = "已为发送短信了喵，查看短信激活账号";

        public static readonly string CreateSucceeded = "已成功创建账号";

        public static readonly string CreateFail = "创建失败";

        public static readonly string Account_Validator = "账号长度需大于4小于25，密码长度需大于6小于25";
    }
}

// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter.SDK.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using GameCloud.Unity.Common;
    using GameCloud.UCenter.Common.Portable.Contracts;
    using GameCloud.UCenter.Common.Portable.Models.AppClient;
    using GameCloud.UCenter.Common.Portable.Models.Ip;
    using GameCloud.UCenter.SDK.Unity;

    public class ClientSampleApp<TDef> : Component<TDef> where TDef : DefSampleApp, new()
    {
        //-------------------------------------------------------------------------
        public override void init()
        {
            EbLog.Note("ClientSampleApp.init()");

            EntityMgr.getDefaultEventPublisher().addHandler(Entity);

            // EtUCenterSDK示例
            var et_ucentersdk = EntityMgr.createEntity<EtUCenterSDK>(null, Entity);
            var co_ucentersdk = et_ucentersdk.getComponent<ClientUCenterSDK<DefUCenterSDK>>();
            co_ucentersdk.UCenterDomain = "http://cragonucenter.chinacloudsites.cn/";
            //co_ucentersdk.UCenterDomain = "http://blair-cs-sh.chinacloudapp.cn/";

            // 获取Ip所在地
            //co_ucentersdk.getIpAddress(_onUCenterGetIpAddress);

            // 获取AppConfig
            co_ucentersdk.getAppConfig("texaspoker", _onUCenterGetAppConfig);

            // 注册
            AccountRegisterInfo register_request = new AccountRegisterInfo();
            register_request.AccountName = "aaaaabbbb";
            register_request.Password = "123456";
            register_request.SuperPassword = "12345678";
            //co_ucentersdk.register(register_request, _onUCenterRegister);

            // 登录
            AccountLoginInfo login_request = new AccountLoginInfo();
            login_request.AccountName = "aaaaabbbb";
            login_request.Password = "123456";
            //co_ucentersdk.login(login_request, _onUCenterLogin);

            // 游客登录
            //co_ucentersdk.guest(_onUCenterGuestLogin);

            // 游客帐号转正
            AccountConvertInfo convert_info = new AccountConvertInfo();
            convert_info.AccountId = "01e94810-ce14-4fff-9c06-16a77990e12c";
            convert_info.AccountName = "asdfg";
            convert_info.OldPassword = "";
            convert_info.Password = "";
            convert_info.SuperPassword = "";
            convert_info.Gender = Gender.DeclineToState;
            convert_info.Name = "";
            convert_info.IdentityNum = "";
            convert_info.PhoneNum = "";
            convert_info.Email = "";
            //co_ucentersdk.convert(convert_info, _onUCenterConvert);

            // 重置密码
            AccountResetPasswordInfo resetpassword_request = new AccountResetPasswordInfo();
            login_request.AccountName = "aaaaabbbb";
            login_request.Password = "123456";
            //co_ucentersdk.resetPassword(resetpassword_request, _onUCenterResetPassword);

            // 上传图片
            //string account_id = "1111";
            //byte[] buffer = new byte[100];
            //MemoryStream ms = new MemoryStream(buffer);
            //co_ucentersdk.uploadProfileImage(account_id, ms, _onUCenterUploadProfileImage);
        }

        //-------------------------------------------------------------------------
        public override void release()
        {
            EbLog.Note("ClientSampleApp.release()");
        }

        //-------------------------------------------------------------------------
        public override void update(float elapsed_tm)
        {
        }

        //-------------------------------------------------------------------------
        public override void handleEvent(object sender, EntityEvent e)
        {
        }

        //-------------------------------------------------------------------------
        void _onUCenterGetIpAddress(UCenterResponseStatus status, IPInfoResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterGetIpAddress() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
            else if (status == UCenterResponseStatus.Success)
            {
                EbLog.Note("Code=" + response.Code);
                EbLog.Note("Area=" + response.Content.Area);
                EbLog.Note("City=" + response.Content.City);
                EbLog.Note("Country=" + response.Content.Country);
                EbLog.Note("Region=" + response.Content.Region);
                EbLog.Note("IP=" + response.Content.IP);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterGetAppConfig(UCenterResponseStatus status, AppConfigurationResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterGetAppConfig() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
            else if (status == UCenterResponseStatus.Success)
            {
                EbLog.Note("AppId=" + response.AppId);
                EbLog.Note("Configuration=" + response.Configuration);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterRegister(UCenterResponseStatus status, AccountRegisterResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterRegister() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterLogin(UCenterResponseStatus status, AccountLoginResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterLogin() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterGuestLogin(UCenterResponseStatus status, AccountGuestLoginResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterGuestLogin() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
            else
            {
                EbLog.Note("AccountId=" + response.AccountId);
                EbLog.Note("AccountName=" + response.AccountName);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterConvert(UCenterResponseStatus status, AccountConvertResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterConvert() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
            //else
            //{
            //    EbLog.Note("AccountId=" + response.AccountId);
            //    EbLog.Note("AccountName=" + response.AccountName);
            //}
        }

        //-------------------------------------------------------------------------
        void _onUCenterResetPassword(UCenterResponseStatus status, AccountResetPasswordResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterResetPassword() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
        }

        //-------------------------------------------------------------------------
        void _onUCenterUploadProfileImage(UCenterResponseStatus status, AccountUploadProfileImageResponse response, UCenterError error)
        {
            EbLog.Note("ClientSampleApp._onUCenterUploadProfileImage() UCenterResult=" + status);

            if (error != null)
            {
                EbLog.Note("ErrorCode=" + error.ErrorCode);
                EbLog.Note("ErrorMessage=" + error.Message);
            }
        }
    }
}

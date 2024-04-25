## 简介

一个小型的视频网站后台，项目是学习.net6期间开发的。包含了用户，视频，弹幕。用到了ffmpeg，sqlserver，signalr，mongdb，redis。后台任务视频转码和分片，包含了hls加密解密。前端地址https://github.com/NLick47/VideoStation

### 运行环境

如果你想运行这个项目的话，请确保安装了mongdb，sqlserver，redis服务。你可以使用efcore的数据迁移来生成表。在此时之前，你需要在数据库中创建一张配置表

```sql
    CREATE TABLE [dbo].[T_Configs](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Name] [varchar](150) NOT NULL,
        [Value] [varchar](max) NOT NULL,
     CONSTRAINT [PK_T_Configs] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

```

并插入以下数据

```sql
SET IDENTITY_INSERT [dbo].[T_Configs] ON 

INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (1, N'Cors', N'{"Origins":["http://localhost:9773"]}')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (2, N'Redis', N'{"ConnStr":"127.0.0.1:6379"}')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (3, N'JWT', N'{"Issuer": "nick", "Audience": "nick", "Key": "q;sd''asdp2130//;", "ExpireSeconds":24192000 }')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (4, N'SendCloudEmail', N'{"smtp":[{"ApiKey":"","From":"@qq.com","ApiUser":"smtp.qq.com"},{"ApiKey":"","From":"@163.com","ApiUser":"smtp.163.com"}]}')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (5, N'FileServiceOptions', N'{"RootPath":"d:\\desk\\videos","VideoExtensions":[".mp4",".flv",".avi",".flv"],"ImgExtensions":[".png",".jpg"],"VideoMaxSize":524288000,"ImagingMaxSize":10485760}')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (15, N'ValidCodeJWT', N'{"Issuer": "nick", "Audience": "nick", "Key": "q;sd''asdp2130//da123@*ASdasQSFHASDJJs;;", "ExpireSeconds":24,192,000 }')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (16, N'MongoDbSettings', N'{
"ConnectionString":"mongodb://localhost:27017",
"DatabaseName":"bi"
}')
INSERT [dbo].[T_Configs] ([Id], [Name], [Value]) VALUES (17, N'defaultAvatar', N'https://q7.itc.cn/q_70/images03/20240405/bd739544ef6c44cba8aed4fbc0fcc967.jpeg')
SET IDENTITY_INSERT [dbo].[T_Configs] OFF

```

最后这是我一部分nginx的配置

```nginx
  server {
		client_max_body_size 15M;
        listen       8080;
        server_name  localhost;
 	location / {
        root   html;
        index  index.html;
    }
    
    //该为hls切片和视频封面的路径
    location /static{
        add_header 'Access-Control-Allow-Origin' '*';
   		add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS,PUT';
 		alias  D:/desk/videos;
	}
	
	location /api/danmakuHub{
		add_header 'Access-Control-Allow-Origin' '*';
   		add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS,PUT';
        add_header 'Access-Control-Allow-Headers' 'Authorization, Content-Type';
 		proxy_pass http://localhost:28428;  
        proxy_set_header Host $host; 
        proxy_set_header X-Real-IP $remote_addr; 
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;  
   }
    
	location /api/danmaku{
		add_header 'Access-Control-Allow-Origin' '*';
   		add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS,PUT';
        add_header 'Access-Control-Allow-Headers' 'Authorization, Content-Type';
 		proxy_pass http://localhost:5176;  # 转发到后端服务器地址及端口
        proxy_set_header Host $host; 
        proxy_set_header X-Real-IP $remote_addr; 
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;  
		}
		
		
	location /api/middle{
        add_header 'Access-Control-Allow-Origin' '*';
   	    add_header 'Access-Control-Allow-Methods' 'GET, POST, OPTIONS,PUT';
        add_header 'Access-Control-Allow-Headers' 'Authorization, Content-Type';
 	    proxy_pass http://localhost:5297;  
        proxy_set_header Host $host; 
        proxy_set_header X-Real-IP $remote_addr; 
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }
      //......按照后台接口配置完
    }

```


using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetSociety.API.Migrations
{
    /// <inheritdoc />
    public partial class AddResetToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 在 Up(MigrationBuilder migrationBuilder) 方法裡面

            // 只新增 PasswordResetToken 欄位
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            // 只新增 ResetTokenExpires 欄位
            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpires",
                table: "Members",
                type: "datetime2",
                nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "AcadamyBadge",
            //    columns: table => new
            //    {
            //        BadgeId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        BadgeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AcadamyBadge", x => x.BadgeId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AreaDetails",
            //    columns: table => new
            //    {
            //        AreaId = table.Column<byte>(type: "tinyint", nullable: false),
            //        AreaName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AreaDetails", x => x.AreaId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ArticleTags",
            //    columns: table => new
            //    {
            //        ArticleID = table.Column<int>(type: "int", nullable: false),
            //        TagID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Categories",
            //    columns: table => new
            //    {
            //        CategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Categories", x => x.CategoryId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CertificateTemplate",
            //    columns: table => new
            //    {
            //        TemplateId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        BackgroundImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        TextLayoutJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CertificateTemplate", x => x.TemplateId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Coupons",
            //    columns: table => new
            //    {
            //        CouponId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        MinPurchase = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Coupons", x => x.CouponId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseCategories",
            //    columns: table => new
            //    {
            //        CategoryId = table.Column<byte>(type: "tinyint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseCategories", x => x.CategoryId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseInstructor",
            //    columns: table => new
            //    {
            //        InstructorId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Phone = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
            //        Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseInstructor", x => x.InstructorId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EmailTypes",
            //    columns: table => new
            //    {
            //        TypeId = table.Column<int>(type: "int", nullable: false),
            //        TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmailTypes", x => x.TypeId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ForumTags",
            //    columns: table => new
            //    {
            //        TagId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TagName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tags", x => x.TagId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Members",
            //    columns: table => new
            //    {
            //        MemberId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
            //        RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
            //        LastloginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ProfilePic = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        LoginMethod = table.Column<int>(type: "int", nullable: true),
            //        VerificationCode = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
            //        LoginCount = table.Column<int>(type: "int", nullable: false),
            //        LoginFailCount = table.Column<int>(type: "int", nullable: false),
            //        Role = table.Column<int>(type: "int", nullable: false),
            //        PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Members_1", x => x.MemberId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductCategories",
            //    columns: table => new
            //    {
            //        CategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductCategories", x => x.CategoryId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TagCategories",
            //    columns: table => new
            //    {
            //        TagCategoryId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TagCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__MarkCate__122397E701691D6B", x => x.TagCategoryId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TicketType",
            //    columns: table => new
            //    {
            //        TicketTypeId = table.Column<int>(type: "int", nullable: false),
            //        TypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TicketType", x => x.TicketTypeId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserBadge",
            //    columns: table => new
            //    {
            //        UserBadgeId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        BadgeId = table.Column<int>(type: "int", nullable: false),
            //        GrantedTime = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserBadge", x => x.UserBadgeId);
            //        table.ForeignKey(
            //            name: "FK_UserBadge_AcadamyBadge",
            //            column: x => x.BadgeId,
            //            principalTable: "AcadamyBadge",
            //            principalColumn: "BadgeId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Courses",
            //    columns: table => new
            //    {
            //        CourseId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CategoryId = table.Column<byte>(type: "tinyint", nullable: false),
            //        Type = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Courses", x => x.CourseId);
            //        table.ForeignKey(
            //            name: "FK_Courses_CourseCategories",
            //            column: x => x.CategoryId,
            //            principalTable: "CourseCategories",
            //            principalColumn: "CategoryId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EmailLog",
            //    columns: table => new
            //    {
            //        EmailId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        EmailType = table.Column<int>(type: "int", nullable: false),
            //        ReceiverEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //        Status = table.Column<bool>(type: "bit", nullable: false),
            //        ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RetryCount = table.Column<byte>(type: "tinyint", nullable: true),
            //        CreatedTime = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmailLog", x => x.EmailId);
            //        table.ForeignKey(
            //            name: "FK_EmailLog_EmailTypes",
            //            column: x => x.EmailType,
            //            principalTable: "EmailTypes",
            //            principalColumn: "TypeId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Activities",
            //    columns: table => new
            //    {
            //        ActivityId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        MaxCapacity = table.Column<int>(type: "int", nullable: false),
            //        RegistrationStartTime = table.Column<DateTime>(type: "datetime", nullable: false),
            //        RegistrationEndTime = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Status = table.Column<byte>(type: "tinyint", nullable: false),
            //        Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
            //        Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ActivityImg = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //        ActivityCheck = table.Column<byte>(type: "tinyint", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Activiti__45F4A7F1CE963BFF", x => x.ActivityId);
            //        table.ForeignKey(
            //            name: "FK_Activities_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Articles",
            //    columns: table => new
            //    {
            //        ArticleId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        TagId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        PostDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        LastUpdate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Like = table.Column<int>(type: "int", nullable: true),
            //        DisLike = table.Column<int>(type: "int", nullable: true),
            //        Popular = table.Column<int>(type: "int", nullable: true),
            //        CommentCount = table.Column<int>(type: "int", nullable: true),
            //        Picture = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Articles", x => x.ArticleId);
            //        table.ForeignKey(
            //            name: "FK_Articles_Categories",
            //            column: x => x.CategoryId,
            //            principalTable: "Categories",
            //            principalColumn: "CategoryId");
            //        table.ForeignKey(
            //            name: "FK_Articles_ForumTags",
            //            column: x => x.TagId,
            //            principalTable: "ForumTags",
            //            principalColumn: "TagId");
            //        table.ForeignKey(
            //            name: "FK_Articles_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Favorites",
            //    columns: table => new
            //    {
            //        FavoriteId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        TargetId = table.Column<int>(type: "int", nullable: false),
            //        TargetType = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Favorites_1", x => x.FavoriteId);
            //        table.ForeignKey(
            //            name: "FK_Favorites_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Landmarks",
            //    columns: table => new
            //    {
            //        LandmarkId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
            //        Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
            //        Category = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        MemberId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Landmark__3195B57FDCCBC1D9", x => x.LandmarkId);
            //        table.ForeignKey(
            //            name: "FK_Landmarks_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Notifications",
            //    columns: table => new
            //    {
            //        NotificationId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        TypeId = table.Column<int>(type: "int", nullable: true),
            //        CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
            //        ReadTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Notifications", x => x.NotificationId);
            //        table.ForeignKey(
            //            name: "FK_Notifications_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserAddress",
            //    columns: table => new
            //    {
            //        AddressId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ReceiverName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        ReceiverPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        IsDefault = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserAddress", x => x.AddressId);
            //        table.ForeignKey(
            //            name: "FK_UserAddress_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserCoupons",
            //    columns: table => new
            //    {
            //        UserCouponId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        CouponId = table.Column<int>(type: "int", nullable: false),
            //        IsUsed = table.Column<bool>(type: "bit", nullable: false),
            //        UsedDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserCoupons", x => x.UserCouponId);
            //        table.ForeignKey(
            //            name: "FK_UserCoupons_Coupons",
            //            column: x => x.CouponId,
            //            principalTable: "Coupons",
            //            principalColumn: "CouponId");
            //        table.ForeignKey(
            //            name: "FK_UserCoupons_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Products",
            //    columns: table => new
            //    {
            //        ProductId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CategoryId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ProductName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        Stock = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Products", x => x.ProductId);
            //        table.ForeignKey(
            //            name: "FK_Products_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_Products_ProductCategories",
            //            column: x => x.CategoryId,
            //            principalTable: "ProductCategories",
            //            principalColumn: "CategoryId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ActivityTags",
            //    columns: table => new
            //    {
            //        TagId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TagType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        TagCategoryId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Marks__4E30D34617074FDA", x => x.TagId);
            //        table.ForeignKey(
            //            name: "FK_ActivityTags_TagCategories",
            //            column: x => x.TagCategoryId,
            //            principalTable: "TagCategories",
            //            principalColumn: "TagCategoryId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseDetails",
            //    columns: table => new
            //    {
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        AreaId = table.Column<byte>(type: "tinyint", nullable: true),
            //        InstructorId = table.Column<int>(type: "int", nullable: true),
            //        Price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        Status = table.Column<byte>(type: "tinyint", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
            //        EndDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())"),
            //        ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseDetail", x => x.CourseDetailId);
            //        table.ForeignKey(
            //            name: "FK_CourseDetails_AreaDetails",
            //            column: x => x.AreaId,
            //            principalTable: "AreaDetails",
            //            principalColumn: "AreaId");
            //        table.ForeignKey(
            //            name: "FK_CourseDetails_CourseInstructor",
            //            column: x => x.InstructorId,
            //            principalTable: "CourseInstructor",
            //            principalColumn: "InstructorId");
            //        table.ForeignKey(
            //            name: "FK_CourseDetails_Courses",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ActivityAuditLog",
            //    columns: table => new
            //    {
            //        LogId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ActivityId = table.Column<int>(type: "int", nullable: false),
            //        previous_status = table.Column<int>(type: "int", nullable: true),
            //        new_status = table.Column<int>(type: "int", nullable: false),
            //        review_comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //        created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Activity__9E2397E0A647D286", x => x.LogId);
            //        table.ForeignKey(
            //            name: "FK_ActivityAuditLog_Activities",
            //            column: x => x.ActivityId,
            //            principalTable: "Activities",
            //            principalColumn: "ActivityId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Participants",
            //    columns: table => new
            //    {
            //        ParticipantId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ActivityId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TicketTypeId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Particip__7227997E3252CD6F", x => x.ParticipantId);
            //        table.ForeignKey(
            //            name: "FK_Participants_Activities",
            //            column: x => x.ActivityId,
            //            principalTable: "Activities",
            //            principalColumn: "ActivityId");
            //        table.ForeignKey(
            //            name: "FK_Participants_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_Participants_TicketType",
            //            column: x => x.TicketTypeId,
            //            principalTable: "TicketType",
            //            principalColumn: "TicketTypeId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Tickets",
            //    columns: table => new
            //    {
            //        ActivityId = table.Column<int>(type: "int", nullable: false),
            //        TicketTypeId = table.Column<int>(type: "int", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "FK_Tickets_Activities",
            //            column: x => x.ActivityId,
            //            principalTable: "Activities",
            //            principalColumn: "ActivityId");
            //        table.ForeignKey(
            //            name: "FK_Tickets_TicketType",
            //            column: x => x.TicketTypeId,
            //            principalTable: "TicketType",
            //            principalColumn: "TicketTypeId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Collectibles",
            //    columns: table => new
            //    {
            //        CollectibleId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ArticleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "FK_Collectibles_Articles",
            //            column: x => x.ArticleId,
            //            principalTable: "Articles",
            //            principalColumn: "ArticleId");
            //        table.ForeignKey(
            //            name: "FK_Collectibles_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Comments",
            //    columns: table => new
            //    {
            //        CommentId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ArticleId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        Content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        PostDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Picture = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Comments", x => x.CommentId);
            //        table.ForeignKey(
            //            name: "FK_Comments_Articles",
            //            column: x => x.ArticleId,
            //            principalTable: "Articles",
            //            principalColumn: "ArticleId");
            //        table.ForeignKey(
            //            name: "FK_Comments_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Videos",
            //    columns: table => new
            //    {
            //        VideoId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ArticleId = table.Column<int>(type: "int", nullable: false),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: true),
            //        VideoURL = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "FK_Videos_Articles",
            //            column: x => x.ArticleId,
            //            principalTable: "Articles",
            //            principalColumn: "ArticleId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LandmarkComments",
            //    columns: table => new
            //    {
            //        CommentID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        LandmarkID = table.Column<int>(type: "int", nullable: false),
            //        LandmarkComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
            //        MemberId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Landmark__C3B4DFAA704EAD1F", x => x.CommentID);
            //        table.ForeignKey(
            //            name: "FK_LandmarkComments_Landmarks",
            //            column: x => x.LandmarkID,
            //            principalTable: "Landmarks",
            //            principalColumn: "LandmarkId");
            //        table.ForeignKey(
            //            name: "FK_LandmarkComments_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Orders",
            //    columns: table => new
            //    {
            //        OrderId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        UserCouponId = table.Column<int>(type: "int", nullable: false),
            //        DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        FinalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        OrderDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
            //        ShippingAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Orders", x => x.OrderId);
            //        table.ForeignKey(
            //            name: "FK_Orders_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_Orders_UserCoupons",
            //            column: x => x.UserCouponId,
            //            principalTable: "UserCoupons",
            //            principalColumn: "UserCouponId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductFavorites",
            //    columns: table => new
            //    {
            //        FavoriteId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductFavorites", x => x.FavoriteId);
            //        table.ForeignKey(
            //            name: "FK_ProductFavorites_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_ProductFavorites_Products",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "ProductId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductImages",
            //    columns: table => new
            //    {
            //        ImageId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductImages", x => x.ImageId);
            //        table.ForeignKey(
            //            name: "FK_ProductImages_Products",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "ProductId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductReviews",
            //    columns: table => new
            //    {
            //        ReviewId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        Rating = table.Column<int>(type: "int", nullable: false),
            //        Comment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            //        ReviewDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductReviews", x => x.ReviewId);
            //        table.ForeignKey(
            //            name: "FK_ProductReviews_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_ProductReviews_Products",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "ProductId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ShoppingCart",
            //    columns: table => new
            //    {
            //        CartId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ShoppingCart", x => x.CartId);
            //        table.ForeignKey(
            //            name: "FK_ShoppingCart_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //        table.ForeignKey(
            //            name: "FK_ShoppingCart_Products",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "ProductId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ActivityTagsMappings",
            //    columns: table => new
            //    {
            //        ActivityId = table.Column<int>(type: "int", nullable: false),
            //        TagId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "FK_ActivityTagsMappings_Activities",
            //            column: x => x.ActivityId,
            //            principalTable: "Activities",
            //            principalColumn: "ActivityId");
            //        table.ForeignKey(
            //            name: "FK_ActivityTagsMappings_ActivityTags",
            //            column: x => x.TagId,
            //            principalTable: "ActivityTags",
            //            principalColumn: "TagId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LandmarkTags",
            //    columns: table => new
            //    {
            //        LandmarkId = table.Column<int>(type: "int", nullable: false),
            //        TagId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.ForeignKey(
            //            name: "FK_LandmarkTags_ActivityTags",
            //            column: x => x.TagId,
            //            principalTable: "ActivityTags",
            //            principalColumn: "TagId");
            //        table.ForeignKey(
            //            name: "FK_LandmarkTags_Landmarks",
            //            column: x => x.LandmarkId,
            //            principalTable: "Landmarks",
            //            principalColumn: "LandmarkId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseChapters",
            //    columns: table => new
            //    {
            //        ChapterId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false),
            //        ChapterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SortOrder = table.Column<int>(type: "int", nullable: false),
            //        Duration = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseChapters", x => x.ChapterId);
            //        table.ForeignKey(
            //            name: "FK_CourseChapters_CourseDetails",
            //            column: x => x.CourseDetailId,
            //            principalTable: "CourseDetails",
            //            principalColumn: "CourseDetailId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseOrders",
            //    columns: table => new
            //    {
            //        OrderId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false),
            //        BuyerUserId = table.Column<int>(type: "int", nullable: false),
            //        IsGift = table.Column<bool>(type: "bit", nullable: false),
            //        ReceiverEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
            //        ReceiverName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        OrderedDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())"),
            //        Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        TransactionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseOrders", x => x.OrderId);
            //        table.ForeignKey(
            //            name: "FK_CourseOrders_CourseDetails",
            //            column: x => x.CourseDetailId,
            //            principalTable: "CourseDetails",
            //            principalColumn: "CourseDetailId");
            //        table.ForeignKey(
            //            name: "FK_CourseOrders_Members",
            //            column: x => x.BuyerUserId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseReviews",
            //    columns: table => new
            //    {
            //        ReviewId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Rating = table.Column<byte>(type: "tinyint", nullable: false),
            //        CreatedTime = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseReviews", x => x.ReviewId);
            //        table.ForeignKey(
            //            name: "FK_CourseReviews_CourseDetails",
            //            column: x => x.CourseDetailId,
            //            principalTable: "CourseDetails",
            //            principalColumn: "CourseDetailId");
            //        table.ForeignKey(
            //            name: "FK_CourseReviews_Members",
            //            column: x => x.UserId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserCertificate",
            //    columns: table => new
            //    {
            //        CertificateId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false),
            //        TemplateId = table.Column<int>(type: "int", nullable: true),
            //        IssueDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserCertificate", x => x.CertificateId);
            //        table.ForeignKey(
            //            name: "FK_UserCertificate_CertificateTemplate",
            //            column: x => x.TemplateId,
            //            principalTable: "CertificateTemplate",
            //            principalColumn: "TemplateId");
            //        table.ForeignKey(
            //            name: "FK_UserCertificate_CourseDetails",
            //            column: x => x.CourseDetailId,
            //            principalTable: "CourseDetails",
            //            principalColumn: "CourseDetailId");
            //        table.ForeignKey(
            //            name: "FK_UserCertificate_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Payments",
            //    columns: table => new
            //    {
            //        PaymentId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ParticipantId = table.Column<int>(type: "int", nullable: false),
            //        Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
            //        TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        PaymentMethod = table.Column<byte>(type: "tinyint", nullable: false),
            //        PaymentDate = table.Column<DateTime>(type: "datetime", nullable: false),
            //        Status = table.Column<byte>(type: "tinyint", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Payments__9B556A588ABCE3B9", x => x.PaymentId);
            //        table.ForeignKey(
            //            name: "FK_Payments_Participants",
            //            column: x => x.ParticipantId,
            //            principalTable: "Participants",
            //            principalColumn: "ParticipantId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OrderDetails",
            //    columns: table => new
            //    {
            //        OrderDetailId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OrderId = table.Column<int>(type: "int", nullable: false),
            //        ProductId = table.Column<int>(type: "int", nullable: false),
            //        Quantity = table.Column<int>(type: "int", nullable: false),
            //        UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailId);
            //        table.ForeignKey(
            //            name: "FK_OrderDetails_Orders",
            //            column: x => x.OrderId,
            //            principalTable: "Orders",
            //            principalColumn: "OrderId");
            //        table.ForeignKey(
            //            name: "FK_OrderDetails_Products",
            //            column: x => x.ProductId,
            //            principalTable: "Products",
            //            principalColumn: "ProductId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OrderLogs",
            //    columns: table => new
            //    {
            //        OrderLogId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        OrderId = table.Column<int>(type: "int", nullable: false),
            //        LogType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
            //        LogDate = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: false, defaultValueSql: "(sysdatetime())"),
            //        Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
            //        Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        TrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        Remark = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderLogs", x => x.OrderLogId);
            //        table.ForeignKey(
            //            name: "FK_OrderLogs_Orders",
            //            column: x => x.OrderId,
            //            principalTable: "Orders",
            //            principalColumn: "OrderId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChapterQuiz",
            //    columns: table => new
            //    {
            //        QuizId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ChapterId = table.Column<int>(type: "int", nullable: true),
            //        Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChapterQuiz", x => x.QuizId);
            //        table.ForeignKey(
            //            name: "FK_ChapterQuiz_CourseChapters",
            //            column: x => x.ChapterId,
            //            principalTable: "CourseChapters",
            //            principalColumn: "ChapterId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChapterVideo",
            //    columns: table => new
            //    {
            //        VideoId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: true),
            //        ChapterId = table.Column<int>(type: "int", nullable: false),
            //        VideoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChapterVideo", x => x.VideoId);
            //        table.ForeignKey(
            //            name: "FK_ChapterVideo_CourseChapters",
            //            column: x => x.ChapterId,
            //            principalTable: "CourseChapters",
            //            principalColumn: "ChapterId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserCourseRecord",
            //    columns: table => new
            //    {
            //        RecordId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        CourseDetailId = table.Column<int>(type: "int", nullable: false),
            //        LastChapterId = table.Column<int>(type: "int", nullable: false),
            //        UpdatedTime = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserCourseProgress", x => x.RecordId);
            //        table.ForeignKey(
            //            name: "FK_UserCourseRecord_CourseChapters",
            //            column: x => x.LastChapterId,
            //            principalTable: "CourseChapters",
            //            principalColumn: "ChapterId");
            //        table.ForeignKey(
            //            name: "FK_UserCourseRecord_CourseDetails",
            //            column: x => x.CourseDetailId,
            //            principalTable: "CourseDetails",
            //            principalColumn: "CourseDetailId");
            //        table.ForeignKey(
            //            name: "FK_UserCourseRecord_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ChapterQuizRecord",
            //    columns: table => new
            //    {
            //        RecordId = table.Column<int>(type: "int", nullable: false),
            //        MemberId = table.Column<int>(type: "int", nullable: false),
            //        QuizId = table.Column<int>(type: "int", nullable: false),
            //        Score = table.Column<byte>(type: "tinyint", nullable: true),
            //        SubmittedAt = table.Column<DateTime>(type: "datetime2(3)", precision: 3, nullable: true, defaultValueSql: "(sysdatetime())")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ChapterQuizRecord", x => x.RecordId);
            //        table.ForeignKey(
            //            name: "FK_ChapterQuizRecord_ChapterQuiz",
            //            column: x => x.QuizId,
            //            principalTable: "ChapterQuiz",
            //            principalColumn: "QuizId");
            //        table.ForeignKey(
            //            name: "FK_ChapterQuizRecord_Members",
            //            column: x => x.MemberId,
            //            principalTable: "Members",
            //            principalColumn: "MemberId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizQuestion",
            //    columns: table => new
            //    {
            //        QuestionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizId = table.Column<int>(type: "int", nullable: true),
            //        QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //        SortOrder = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizQuestion", x => x.QuestionId);
            //        table.ForeignKey(
            //            name: "FK_QuizQuestion_ChapterQuiz",
            //            column: x => x.QuizId,
            //            principalTable: "ChapterQuiz",
            //            principalColumn: "QuizId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizOption",
            //    columns: table => new
            //    {
            //        OptionId = table.Column<int>(type: "int", nullable: false),
            //        QuestionId = table.Column<int>(type: "int", nullable: true),
            //        OptionText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
            //        IsCorrect = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizOption", x => x.OptionId);
            //        table.ForeignKey(
            //            name: "FK_QuizOption_QuizQuestion",
            //            column: x => x.QuestionId,
            //            principalTable: "QuizQuestion",
            //            principalColumn: "QuestionId");
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Activities_MemberId",
            //    table: "Activities",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ActivityAuditLog_ActivityId",
            //    table: "ActivityAuditLog",
            //    column: "ActivityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ActivityTags_TagCategoryId",
            //    table: "ActivityTags",
            //    column: "TagCategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ActivityTagsMappings_ActivityId",
            //    table: "ActivityTagsMappings",
            //    column: "ActivityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ActivityTagsMappings_TagId",
            //    table: "ActivityTagsMappings",
            //    column: "TagId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Articles_CategoryId",
            //    table: "Articles",
            //    column: "CategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Articles_MemberId",
            //    table: "Articles",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Articles_TagId",
            //    table: "Articles",
            //    column: "TagId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChapterQuiz_ChapterId",
            //    table: "ChapterQuiz",
            //    column: "ChapterId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChapterQuizRecord_MemberId",
            //    table: "ChapterQuizRecord",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChapterQuizRecord_QuizId",
            //    table: "ChapterQuizRecord",
            //    column: "QuizId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ChapterVideo_ChapterId",
            //    table: "ChapterVideo",
            //    column: "ChapterId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Collectibles_ArticleId",
            //    table: "Collectibles",
            //    column: "ArticleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Collectibles_MemberId",
            //    table: "Collectibles",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_ArticleId",
            //    table: "Comments",
            //    column: "ArticleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Comments_MemberId",
            //    table: "Comments",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseChapters_CourseDetailId",
            //    table: "CourseChapters",
            //    column: "CourseDetailId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseDetails_AreaId",
            //    table: "CourseDetails",
            //    column: "AreaId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseDetails_CourseId",
            //    table: "CourseDetails",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseDetails_InstructorId",
            //    table: "CourseDetails",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseOrders_BuyerUserId",
            //    table: "CourseOrders",
            //    column: "BuyerUserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseOrders_CourseDetailId",
            //    table: "CourseOrders",
            //    column: "CourseDetailId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseReviews_CourseDetailId",
            //    table: "CourseReviews",
            //    column: "CourseDetailId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseReviews_UserId",
            //    table: "CourseReviews",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Courses_CategoryId",
            //    table: "Courses",
            //    column: "CategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EmailLog_EmailType",
            //    table: "EmailLog",
            //    column: "EmailType");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Favorites_MemberId",
            //    table: "Favorites",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LandmarkComments_LandmarkID",
            //    table: "LandmarkComments",
            //    column: "LandmarkID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LandmarkComments_MemberId",
            //    table: "LandmarkComments",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Landmarks_MemberId",
            //    table: "Landmarks",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LandmarkTags_LandmarkId",
            //    table: "LandmarkTags",
            //    column: "LandmarkId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LandmarkTags_TagId",
            //    table: "LandmarkTags",
            //    column: "TagId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Notifications_MemberId",
            //    table: "Notifications",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_OrderId",
            //    table: "OrderDetails",
            //    column: "OrderId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderDetails_ProductId",
            //    table: "OrderDetails",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderLogs_OrderId",
            //    table: "OrderLogs",
            //    column: "OrderId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Orders_MemberId",
            //    table: "Orders",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Orders_UserCouponId",
            //    table: "Orders",
            //    column: "UserCouponId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Participants_ActivityId",
            //    table: "Participants",
            //    column: "ActivityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Participants_MemberId",
            //    table: "Participants",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Participants_TicketTypeId",
            //    table: "Participants",
            //    column: "TicketTypeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Payments_ParticipantId",
            //    table: "Payments",
            //    column: "ParticipantId");

            //migrationBuilder.CreateIndex(
            //    name: "UQ__Payments__55433A4A24F64FFA",
            //    table: "Payments",
            //    column: "TransactionId",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductFavorites_MemberId",
            //    table: "ProductFavorites",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductFavorites_ProductId",
            //    table: "ProductFavorites",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductImages_ProductId",
            //    table: "ProductImages",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductReviews_MemberId",
            //    table: "ProductReviews",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProductReviews_ProductId",
            //    table: "ProductReviews",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_CategoryId",
            //    table: "Products",
            //    column: "CategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_MemberId",
            //    table: "Products",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizOption_QuestionId",
            //    table: "QuizOption",
            //    column: "QuestionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizQuestion_QuizId",
            //    table: "QuizQuestion",
            //    column: "QuizId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShoppingCart_MemberId",
            //    table: "ShoppingCart",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ShoppingCart_ProductId",
            //    table: "ShoppingCart",
            //    column: "ProductId");

            //migrationBuilder.CreateIndex(
            //    name: "UQ__MarkCate__3A5A95C7C690C70A",
            //    table: "TagCategories",
            //    column: "TagCategory",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Tickets_ActivityId",
            //    table: "Tickets",
            //    column: "ActivityId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Tickets_TicketTypeId",
            //    table: "Tickets",
            //    column: "TicketTypeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserAddress_MemberId",
            //    table: "UserAddress",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserBadge_BadgeId",
            //    table: "UserBadge",
            //    column: "BadgeId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCertificate_CourseDetailId",
            //    table: "UserCertificate",
            //    column: "CourseDetailId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCertificate_MemberId",
            //    table: "UserCertificate",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCertificate_TemplateId",
            //    table: "UserCertificate",
            //    column: "TemplateId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCoupons_CouponId",
            //    table: "UserCoupons",
            //    column: "CouponId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCoupons_MemberId",
            //    table: "UserCoupons",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCourseRecord_CourseDetailId",
            //    table: "UserCourseRecord",
            //    column: "CourseDetailId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCourseRecord_LastChapterId",
            //    table: "UserCourseRecord",
            //    column: "LastChapterId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserCourseRecord_MemberId",
            //    table: "UserCourseRecord",
            //    column: "MemberId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Videos_ArticleId",
            //    table: "Videos",
            //    column: "ArticleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityAuditLog");

            migrationBuilder.DropTable(
                name: "ActivityTagsMappings");

            migrationBuilder.DropTable(
                name: "ArticleTags");

            migrationBuilder.DropTable(
                name: "ChapterQuizRecord");

            migrationBuilder.DropTable(
                name: "ChapterVideo");

            migrationBuilder.DropTable(
                name: "Collectibles");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CourseOrders");

            migrationBuilder.DropTable(
                name: "CourseReviews");

            migrationBuilder.DropTable(
                name: "EmailLog");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "LandmarkComments");

            migrationBuilder.DropTable(
                name: "LandmarkTags");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderLogs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductFavorites");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "QuizOption");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "UserBadge");

            migrationBuilder.DropTable(
                name: "UserCertificate");

            migrationBuilder.DropTable(
                name: "UserCourseRecord");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "EmailTypes");

            migrationBuilder.DropTable(
                name: "ActivityTags");

            migrationBuilder.DropTable(
                name: "Landmarks");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "QuizQuestion");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AcadamyBadge");

            migrationBuilder.DropTable(
                name: "CertificateTemplate");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "TagCategories");

            migrationBuilder.DropTable(
                name: "UserCoupons");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "TicketType");

            migrationBuilder.DropTable(
                name: "ChapterQuiz");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "ForumTags");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "CourseChapters");

            migrationBuilder.DropTable(
                name: "CourseDetails");

            migrationBuilder.DropTable(
                name: "AreaDetails");

            migrationBuilder.DropTable(
                name: "CourseInstructor");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "CourseCategories");
        }
    }
}

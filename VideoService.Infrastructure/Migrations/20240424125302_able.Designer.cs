﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VideoService.Infrastructure;

#nullable disable

namespace VideoService.Infrastructure.Migrations
{
    [DbContext(typeof(VideoDbContext))]
    [Migration("20240424125302_able")]
    partial class able
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("VideoService.Domain.Entities.Collection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreateUserId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.CollectionItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<long?>("CollectionId")
                        .HasColumnType("bigint");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CollectionId");

                    b.HasIndex("VideoId");

                    b.ToTable("CollectionItems");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.CommentReply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("VideoCommentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VideoCommentId");

                    b.ToTable("CommentReply");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.LikeUser", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("id"), 1L, 1);

                    b.Property<int?>("VideoCommentId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("VideoCommentId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.Tourist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("Ip")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Tourists");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CollectCount")
                        .HasColumnType("int");

                    b.Property<string>("CoverUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreateUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LikeCount")
                        .HasColumnType("int");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Transpond")
                        .HasColumnType("int");

                    b.Property<double>("VideoSecond")
                        .HasColumnType("float");

                    b.Property<int>("VideoType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Video");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<int>("LikeCount")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("VideoComment");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoLike", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("VideoLikes");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoResouce", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RawPath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.Property<int>("VideoSize")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("VideoId");

                    b.ToTable("videoResouces");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoTouristLike", b =>
                {
                    b.Property<int>("TouristId")
                        .HasColumnType("int");

                    b.Property<int>("VideoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LikeTime")
                        .HasColumnType("datetime2");

                    b.HasKey("TouristId", "VideoId");

                    b.HasIndex("VideoId");

                    b.ToTable("TouristLikes");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.CollectionItem", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.Collection", "Collection")
                        .WithMany("Item")
                        .HasForeignKey("CollectionId");

                    b.HasOne("VideoService.Domain.Entities.Video", "Video")
                        .WithOne()
                        .HasForeignKey("VideoService.Domain.Entities.CollectionItem", "VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.CommentReply", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.VideoComment", "VideoComment")
                        .WithMany("Replys")
                        .HasForeignKey("VideoCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VideoComment");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.LikeUser", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.VideoComment", null)
                        .WithMany("likeUsers")
                        .HasForeignKey("VideoCommentId");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoComment", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.Video", "Video")
                        .WithMany("Comments")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Video");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoResouce", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.Video", "Video")
                        .WithMany("VideoResouce")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Video");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoTouristLike", b =>
                {
                    b.HasOne("VideoService.Domain.Entities.Tourist", "Tourist")
                        .WithMany("VideoLikes")
                        .HasForeignKey("TouristId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VideoService.Domain.Entities.Video", "Video")
                        .WithMany("TouristLikes")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tourist");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.Collection", b =>
                {
                    b.Navigation("Item");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.Tourist", b =>
                {
                    b.Navigation("VideoLikes");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.Video", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("TouristLikes");

                    b.Navigation("VideoResouce");
                });

            modelBuilder.Entity("VideoService.Domain.Entities.VideoComment", b =>
                {
                    b.Navigation("Replys");

                    b.Navigation("likeUsers");
                });
#pragma warning restore 612, 618
        }
    }
}

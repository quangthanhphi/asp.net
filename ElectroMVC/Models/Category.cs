﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectroMVC.Models
{
    public class Category : CommonAbstract
    {
        public Category()
        {
            this.News = new HashSet<News>();
            this.Posts = new HashSet<Posts>(); // Initialize Posts here
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(150)]
        public string? Title { get; set; }
        public string? Alias { get; set; }

        public string? Description { get; set; }
        [StringLength(150)]
        public string? SeoTitle { get; set; }
        [StringLength(250)]
        public string? SeoDescription { get; set; }
        [StringLength(150)]
        public string? SeoKeywords { get; set; }
        public int Position { get; set; }
        public ICollection<News> News { get; set; }
        public ICollection<Posts> Posts { get; set; }
    }
}


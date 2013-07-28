﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BrewersBuddy.Models
{
	[Table("Friend")]
	public class Friend
	{
		[Key]
		public int FriendId { get; set; }
		[Required]
		public int UserId { get; set; }
		
		[ForeignKey("UserId")]
		public UserProfile User { get; set; }

		public int FriendUserId { get; set; }

	}
}
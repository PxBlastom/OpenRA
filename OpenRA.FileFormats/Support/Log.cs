#region Copyright & License Information
/*
 * Copyright 2007-2011 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace OpenRA
{
	public struct ChannelInfo
	{
		public string Filename;
		public StreamWriter Writer;
	}

	public static class Log
	{
		static string LogPathPrefix = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar;
		public static readonly Dictionary<string, ChannelInfo> Channels = new Dictionary<string, ChannelInfo>();

		public static string LogPath
		{
			get { return LogPathPrefix; }
			set
			{
				LogPathPrefix = value;
				Directory.CreateDirectory(LogPathPrefix);
			}
		}

		static IEnumerable<string> FilenamesForChannel(string channelName, string baseFilename)
		{
			for(var i = 0;; i++ )
				yield return Path.Combine(LogPathPrefix,
					i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
		}

		public static void AddChannel(string channelName, string baseFilename)
		{
			if (Channels.ContainsKey(channelName)) return;

			foreach (var filename in FilenamesForChannel(channelName, baseFilename))
				try
				{
					var writer = File.CreateText(filename);
					writer.AutoFlush = true;

					Channels.Add(channelName,
						new ChannelInfo()
						{
							Filename = filename,
							Writer = writer
						});

					return;
				}
				catch (IOException) { }
		}

		public static void Write(string channel, string format, params object[] args)
		{
			ChannelInfo info;
			if (!Channels.TryGetValue(channel, out info))
				throw new Exception("Tried logging to non-existant channel " + channel);

			info.Writer.WriteLine(format, args);
		}
	}
}

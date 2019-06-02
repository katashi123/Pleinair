﻿// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Pleinair.
//
// Pleinair is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Pleinair is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Pleinair. If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.IO;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair
{
    class Program
    {
        private static IConverter<BinaryFormat, Po> converter;

        static void Main(string[] args)
        {
            Console.WriteLine("Pleinair — A disgaea toolkit for fantranslations by Darkmet98.\nVersion: 1.0");
            Console.WriteLine("Thanks to Pleonex for the Yarhl libraries.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 1 && args.Length != 2 && args.Length != 3)
            {
                Console.WriteLine("\nUsage: Pleinar.exe <-export/-import/-export_elf>");
                Console.WriteLine("\nTALK.DAT");
                Console.WriteLine("Export TALK.DAT to Po: Pleinair.exe -export_talkdat \"TALK.DAT\"");
                Console.WriteLine("Import Po to TALK.DAT: Pleinair.exe -import_talkdat \"TALK.po\" \"TALK.DAT\"");
                Console.WriteLine("\nANOTHER DAT");
                Console.WriteLine("Export CHAR_E.DAT to Po: Pleinair.exe -export_dat \"CHAR_E.DAT\"");
                //Console.WriteLine("Import Po to TALK.DAT: Pleinair.exe -import_dat \"TALK.po\" \"TALK.DAT\"");
                Console.WriteLine("\nExecutable");
                Console.WriteLine("Dump the dis1_st.exe's strings to Po: Pleinair.exe -export_elf \"dis1_st.exe\"");
                return;
            }
            switch (args[0])
            {
                case "-export_talkdat":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        converter = new TALKDAT.Binary2Po { };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".pot");
                    }
                    break;
                case "-import_talkdat":
                    if (File.Exists(args[1]) && File.Exists(args[2]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        TALKDAT.po2Binary P2B = new TALKDAT.po2Binary
                        {
                            OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                            {
                                DefaultEncoding = new UTF8Encoding(),
                                Endianness = EndiannessMode.LittleEndian,
                            }
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(P2B);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.DAT");
                    }
                    break;
                case "-export_elf":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        converter = new ELF.Binary2Po { };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);
                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(file + ".pot");
                    }
                    break;
                case "-import_elf":
                    if (File.Exists(args[1]))
                    {

                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        ELF.po2Binary importer = new ELF.po2Binary
                        {
                            OriginalFile = new DataReader(new DataStream(args[2], FileOpenMode.Read))
                            {
                                DefaultEncoding = new UTF8Encoding(),
                                Endianness = EndiannessMode.LittleEndian,
                            }
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoDat = nodo.Transform<Po, BinaryFormat>(importer);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 4);
                        nodoDat.Stream.WriteTo(file + "_new.exe");
                    }
                    break;
                case "-export_dat":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        switch(Path.GetFileName(args[1]).ToUpper())
                        {
                            case "CHAR_E.DAT":
                                converter = new DAT.Binary2po_CHAR_E { };
                                break;
                            case "CHARHELP.DAT":
                                converter = new DAT.Binary2po_CHARHELP { };
                                break;
                        }

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>().Stream.WriteTo(Path.GetFileName(args[1]) + ".pot");
                    }
                    break;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace MMD
{
    /// <summary>
    /// Loads a .pmx/.pmd model (Note, only 2.0 is supported)
    /// </summary>
    public class Load
    {
        /// <summary>
        /// "PMX ", "PMD "
        /// </summary>
        private string _signature;
        /// <summary>
        /// 2.0, 2.1
        /// </summary>
        private float _version;
        /// <summary>
        /// Fixed at 8 in PMX 2.0
        /// </summary>
        private byte _globals;
        /// <summary>
        /// UTF16, UTF8 -> [0, 1]
        /// </summary>
        private byte _textEncoding;
        private byte _appendixUv;
        private byte _vertexIndexSize;
        private byte _textureIndexSize;
        private byte _materialIndexSize;
        private byte _boneIndexSize;
        private byte _morphIndexSize;
        private byte _rigidbodyIndexSize;
        private string _modelNameLocal;
        private string _modelNameUniversal;
        private string _commentLocal;
        private string _commentUniversal;
        private int _vertexcount;
        
        private List<Vertex> _vertices = new List<Vertex>();
        private int _surfaceCount;
        private List<Surface> _surfaces = new List<Surface>();
        private int _textureCount;
        private List<string> _textures = new List<string>();
        private int MaterialCount;
        private List<Material> _materials = new List<Material>();

        private readonly List<string> _license = new List<string>();
        public Load(string path)
        {
            foreach (var s in Directory.GetFiles(path.Substring(0, path.LastIndexOf('\\')))
                .Where(e => e.EndsWith("license"))) License.Add(s);
            var fileStream = new FileStream(path, FileMode.Open);
            var binaryReader = new BinaryReader(fileStream, Encoding.Unicode);
            _signature = Encoding.UTF8.GetString(binaryReader.ReadBytes(4));
            _version = binaryReader.ReadSingle();
            _globals = binaryReader.ReadByte();
            TextEncoding = binaryReader.ReadByte();
            AppendixUv = binaryReader.ReadByte();
            VertexIndexSize = binaryReader.ReadByte();
            TextureIndexSize = binaryReader.ReadByte();
            MaterialIndexSize = binaryReader.ReadByte();
            BoneIndexSize = binaryReader.ReadByte();
            MorphIndexSize = binaryReader.ReadByte();
            RigidbodyIndexSize = binaryReader.ReadByte();

            #region Encoding
            if (TextEncoding == 0)
            {
                int i;
                i = binaryReader.ReadInt32();
                ModelNameLocal = Encoding.Unicode.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                ModelNameUniversal = Encoding.Unicode.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                CommentLocal = Encoding.Unicode.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                CommentUniversal = Encoding.Unicode.GetString(binaryReader.ReadBytes(i));
            }else if (TextEncoding == 1)
            {
                int i;
                i = binaryReader.ReadInt32();
                ModelNameLocal = Encoding.UTF8.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                ModelNameUniversal = Encoding.UTF8.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                CommentLocal = Encoding.UTF8.GetString(binaryReader.ReadBytes(i));
                i = binaryReader.ReadInt32();
                CommentUniversal = Encoding.UTF8.GetString(binaryReader.ReadBytes(i));
            }
            else
            {
                throw new Exception("Unknown Encoding Format.");
            }
            #endregion
            
            _vertexcount = binaryReader.ReadInt32();
            for (int i = 0; i < _vertexcount; i++)
            {
                var v = new Vertex();
                v.Position = new Matrix3D<float>()
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle(),
                    Z = binaryReader.ReadSingle()
                };
                v.Normal = new Matrix3D<float>()
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle(),
                    Z = binaryReader.ReadSingle()
                };
                v.UV = new Matrix2D<float>()
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle()
                };
                for (int j = 0; j < AppendixUv; j++)
                {
                    v.Additional_Vec4.Add(binaryReader.ReadSingle());
                }

                v.WeightDeformType = binaryReader.ReadByte();                
                #region WeightDeformType
                if (v.WeightDeformType == 0)
                {
                    if (VertexIndexSize == 1)
                    {
                        v.WeightDeform = new BDEF1()
                        {
                            Index = binaryReader.ReadByte()
                        };
                    }else if (VertexIndexSize == 2)
                    {
                        v.WeightDeform = new BDEF1()
                        {
                            Index = binaryReader.ReadInt16()
                        };
                    }else if (VertexIndexSize == 4)
                    {
                        v.WeightDeform = new BDEF1()
                        {
                            Index = binaryReader.ReadInt16()
                        };
                    }
                }else if (v.WeightDeformType == 1)
                {
                    if (VertexIndexSize == 1)
                    {
                        v.WeightDeform = new BDEF2()
                        {
                            BoneIndex1 = binaryReader.ReadByte(),
                            BoneIndex2 = binaryReader.ReadByte(),
                            BoneWeight1 = binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 2)
                    {
                        v.WeightDeform = new BDEF2()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 4)
                    {
                        v.WeightDeform = new BDEF2()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle()
                        };
                    }

                }else if (v.WeightDeformType == 2)
                {
                    if (VertexIndexSize == 1)
                    {
                        v.WeightDeform = new BDEF4()
                        {
                            BoneIndex1 = binaryReader.ReadByte(),
                            BoneIndex2 = binaryReader.ReadByte(),
                            BoneIndex3 = binaryReader.ReadByte(),
                            BoneIndex4 = binaryReader.ReadByte(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 =  binaryReader.ReadSingle(),
                            BoneWeight3 =  binaryReader.ReadSingle(),
                            BoneWeight4 =  binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 2)
                    {
                        v.WeightDeform = new BDEF4()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneIndex3 = binaryReader.ReadInt16(),
                            BoneIndex4 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 =  binaryReader.ReadSingle(),
                            BoneWeight3 =  binaryReader.ReadSingle(),
                            BoneWeight4 =  binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 4)
                    {
                        v.WeightDeform = new BDEF4()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneIndex3 = binaryReader.ReadInt16(),
                            BoneIndex4 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 =  binaryReader.ReadSingle(),
                            BoneWeight3 =  binaryReader.ReadSingle(),
                            BoneWeight4 =  binaryReader.ReadSingle()
                        };
                    }
                }else if (v.WeightDeformType == 3)
                {
                    if (VertexIndexSize == 1)
                    {
                        v.WeightDeform = new SDEF()
                        {
                            BoneIndex1 = binaryReader.ReadByte(),
                            BoneIndex2 = binaryReader.ReadByte(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            C = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            },
                            R0 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }, 
                            R1 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }
                        };
                    }else if (VertexIndexSize == 2)
                    {
                        v.WeightDeform = new SDEF()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            C = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            },
                            R0 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }, 
                            R1 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }
                        };
                    }else if (VertexIndexSize == 4)
                    {
                        v.WeightDeform = new SDEF()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            C = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            },
                            R0 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }, 
                            R1 = new Matrix3D<float>()
                            {
                                X = binaryReader.ReadSingle(),
                                Y = binaryReader.ReadSingle(),
                                Z = binaryReader.ReadSingle()
                            }
                        };
                    }
                }else if (v.WeightDeformType == 4)
                {
                    if (VertexIndexSize == 1)
                    {
                        v.WeightDeform = new QDEF()
                        {
                            BoneIndex1 = binaryReader.ReadByte(),
                            BoneIndex2 = binaryReader.ReadByte(),
                            BoneIndex3 = binaryReader.ReadByte(),
                            BoneIndex4 = binaryReader.ReadByte(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 = binaryReader.ReadSingle(),
                            BoneWeight3 = binaryReader.ReadSingle(),
                            BoneWeight4 = binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 2)
                    {
                        v.WeightDeform = new QDEF()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneIndex3 = binaryReader.ReadInt16(),
                            BoneIndex4 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 = binaryReader.ReadSingle(),
                            BoneWeight3 = binaryReader.ReadSingle(),
                            BoneWeight4 = binaryReader.ReadSingle()
                        };
                    }else if (VertexIndexSize == 4)
                    {
                        v.WeightDeform = new QDEF()
                        {
                            BoneIndex1 = binaryReader.ReadInt16(),
                            BoneIndex2 = binaryReader.ReadInt16(),
                            BoneIndex3 = binaryReader.ReadInt16(),
                            BoneIndex4 = binaryReader.ReadInt16(),
                            BoneWeight1 = binaryReader.ReadSingle(),
                            BoneWeight2 = binaryReader.ReadSingle(),
                            BoneWeight3 = binaryReader.ReadSingle(),
                            BoneWeight4 = binaryReader.ReadSingle()
                        };
                    }
                }
                
                #endregion
                v.EdgeScale = binaryReader.ReadSingle();
                Vertices.Add(v);
            }


            _surfaceCount = binaryReader.ReadInt32();
            for (int i = 0; i < _surfaceCount / 3; i++)
            {
                if (_vertexIndexSize == 1)
                {
                    var v = new Surface
                    {
                        Obj1 = binaryReader.ReadByte(),
                        Obj2 = binaryReader.ReadByte(),
                        Obj3 = binaryReader.ReadByte()
                    };
                    _surfaces.Add(v);
                }else if (_vertexIndexSize == 2)
                {
                    var v = new Surface
                    {
                        Obj1 = binaryReader.ReadInt16(),
                        Obj2 = binaryReader.ReadInt16(),
                        Obj3 = binaryReader.ReadInt16()
                    };
                    _surfaces.Add(v);
                }else if (_vertexIndexSize == 4)
                {
                    var v = new Surface
                    {
                        Obj1 = binaryReader.ReadInt32(),
                        Obj2 = binaryReader.ReadInt32(),
                        Obj3 = binaryReader.ReadInt32()
                    };
                    _surfaces.Add(v);
                }
            }

            TextureCount = binaryReader.ReadInt32();
            for (int i = 0; i < TextureCount; i++)
            {
                int r = binaryReader.ReadInt32();
                if (_textEncoding == 0)
                {
                    Textures.Add(Encoding.Unicode.GetString(binaryReader.ReadBytes(r)));
                }else if (_textEncoding == 1)
                {
                    Textures.Add(Encoding.UTF8.GetString(binaryReader.ReadBytes(r)));
                }
            }

            MaterialCount = binaryReader.ReadInt32();
            for (int i = 0; i < MaterialCount; i++)
            {
                Material m = new Material();
                int r = binaryReader.ReadInt32();
                m.MaterialNameLocal = _textEncoding == 0
                    ? Encoding.Unicode.GetString(binaryReader.ReadBytes(r))
                    : Encoding.UTF8.GetString(binaryReader.ReadBytes(r));
                r = binaryReader.ReadInt32();
                m.MaterialNameUniversal = _textEncoding == 0
                    ? Encoding.Unicode.GetString(binaryReader.ReadBytes(r))
                    : Encoding.UTF8.GetString(binaryReader.ReadBytes(r));
                Matrix4D<float> matrix4D = new Matrix4D<float>
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle(),
                    Z = binaryReader.ReadSingle(),
                    W = binaryReader.ReadSingle()
                };
                m.DiffuseColor = matrix4D;
                Matrix3D<float> matrix3D = new Matrix3D<float>
                {
                    X = binaryReader.ReadSingle(), Y = binaryReader.ReadSingle(), Z = binaryReader.ReadSingle()
                };
                m.SpecularColor = matrix3D;

                m.SpecularStrength = binaryReader.ReadSingle();
                m.AmbientColor = new Matrix3D<float>()
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle(),
                    Z = binaryReader.ReadSingle()
                };
                var bits = binaryReader.ReadByte();
                m.DrawingFlags = new BitArray(new byte[]{bits});
                m.EdgeColor = new Matrix4D<float>()
                {
                    X = binaryReader.ReadSingle(),
                    Y = binaryReader.ReadSingle(),
                    Z = binaryReader.ReadSingle(),
                    W = binaryReader.ReadSingle()
                };
                m.EdgeScale = binaryReader.ReadSingle();
                if (_materialIndexSize == 1)
                {
                    m.TextureIndex = binaryReader.ReadByte();
                }else if (_materialIndexSize == 2)
                {
                    m.TextureIndex = binaryReader.ReadInt16();
                }else if (_materialIndexSize == 4)
                {
                    m.TextureIndex = binaryReader.ReadInt32();
                }
                
                if (_materialIndexSize == 1)
                {
                    m.EnviromentIndex = binaryReader.ReadByte();
                }else if (_materialIndexSize == 2)
                {
                    m.EnviromentIndex = binaryReader.ReadInt16();
                }else if (_materialIndexSize == 4)
                {
                    m.EnviromentIndex = binaryReader.ReadInt32();
                }

                m.EnviromentBlendMode = binaryReader.ReadByte();
                m.ToonReference = binaryReader.ReadByte();
                if (m.ToonReference == 0)
                {
                    if (_materialIndexSize == 1)
                    {
                        m.ToonValue = binaryReader.ReadByte();
                    }else if (_materialIndexSize == 2)
                    {
                        m.ToonValue = binaryReader.ReadInt16();
                    }else if (_materialIndexSize == 4)
                    {
                        m.ToonValue = binaryReader.ReadInt32();
                    }
                }
                else
                {
                    m.ToonValue = binaryReader.ReadByte();
                }
                
                r = binaryReader.ReadInt32();
                m.MetaData = _textEncoding == 0
                    ? Encoding.Unicode.GetString(binaryReader.ReadBytes(r))
                    : Encoding.UTF8.GetString(binaryReader.ReadBytes(r));
                m.SurfaceCount = binaryReader.ReadInt32();
                _materials.Add(m);
            }
            //Bone Time-- nvm. Ping @rnetiks#7010 to continue.
            
        }

        public int Vertexcount => _vertexcount;

        /// <summary>
        /// UTF16, UTF8 -> [0, 1]
        /// </summary>
        public byte TextEncoding
        {
            get { return _textEncoding; }
            set { _textEncoding = value; }
        }

        public byte AppendixUv
        {
            get { return _appendixUv; }
            set { _appendixUv = value; }
        }

        public byte VertexIndexSize
        {
            get { return _vertexIndexSize; }
            set { _vertexIndexSize = value; }
        }

        public byte TextureIndexSize
        {
            get { return _textureIndexSize; }
            set { _textureIndexSize = value; }
        }

        public byte MaterialIndexSize
        {
            get { return _materialIndexSize; }
            set { _materialIndexSize = value; }
        }

        public byte BoneIndexSize
        {
            get { return _boneIndexSize; }
            set { _boneIndexSize = value; }
        }

        public byte MorphIndexSize
        {
            get { return _morphIndexSize; }
            set { _morphIndexSize = value; }
        }

        public byte RigidbodyIndexSize
        {
            get { return _rigidbodyIndexSize; }
            set { _rigidbodyIndexSize = value; }
        }

        public string ModelNameLocal
        {
            get { return _modelNameLocal; }
            set { _modelNameLocal = value; }
        }

        public string ModelNameUniversal
        {
            get { return _modelNameUniversal; }
            set { _modelNameUniversal = value; }
        }

        public string CommentLocal
        {
            get { return _commentLocal; }
            set { _commentLocal = value; }
        }

        public string CommentUniversal
        {
            get { return _commentUniversal; }
            set { _commentUniversal = value; }
        }

        public List<Vertex> Vertices
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public int SurfaceCount
        {
            get { return _surfaceCount; }
            set { _surfaceCount = value; }
        }
        

        public int TextureCount
        {
            get { return _textureCount; }
            set { _textureCount = value; }
        }

        public List<string> Textures
        {
            get { return _textures; }
            set { _textures = value; }
        }

        public List<Surface> Surfaces => _surfaces;

        public List<string> License
        {
            get { return _license; }
        }

        public List<Material> Materials
        {
            get { return _materials; }
            set { _materials = value; }
        }
    }
}
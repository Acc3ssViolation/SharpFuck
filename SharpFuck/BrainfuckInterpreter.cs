using System;
using System.Collections.Generic;

namespace SharpFuck
{
    class BrainfuckInterpreter
    {
        Stack<int> m_loops;
        int m_instructionPointer;
        int m_dataPointer;
        int m_inputPointer;
        byte[] m_data;
        char[] m_program;
        char[] m_input;
        List<char> m_output;

        public BrainfuckInterpreter()
        {
            m_loops = new Stack<int>();
            m_data = new byte[60000];
            m_output = new List<char>();
        }

        /// <summary>
        /// Runs a brainfuck program.
        /// </summary>
        /// <param name="bf">Brainfuck code</param>
        /// <param name="input">Optional input string</param>
        /// <returns></returns>
        public char[] Run(string bf, string input = null)
        {
            m_program = bf.ToCharArray();
            m_input = input?.ToCharArray();
            m_output.Clear();
            m_inputPointer = 0;
            m_instructionPointer = 0;
            m_dataPointer = 0;
            for(int i = 0; i < m_data.Length; i++)
                m_data[i] = 0;

            while(m_instructionPointer < m_program.Length)
            {
                ParseChar(m_program[m_instructionPointer]);
            }

            return m_output.ToArray();
        }

        /// <summary>
        /// Sets up the interpreter for step by step execution.
        /// </summary>
        /// <param name="bf">Brainfuck code</param>
        /// <param name="input">Optional input string</param>
        public void RunStepped(string bf, string input = null)
        {
            m_program = bf.ToCharArray();
            m_input = input?.ToCharArray();
            m_output.Clear();
            m_inputPointer = 0;
            m_instructionPointer = 0;
            m_dataPointer = 0;
            for(int i = 0; i < m_data.Length; i++)
                m_data[i] = 0;
        }

        /// <summary>
        /// Parses a single character. Returns false when the program is finished.
        /// </summary>
        /// <returns></returns>
        public bool DoStep()
        {
            if(m_instructionPointer < m_program.Length)
            {
                ParseChar(m_program[m_instructionPointer]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the current state of the output.
        /// </summary>
        /// <returns></returns>
        public char[] GetOutput()
        {
            return m_output.ToArray();
        }

        private void ParseChar(char c)
        {
            if(c == '>')
            {
                m_dataPointer++;
                m_instructionPointer++;
            }
            else if(c == '<')
            {
                m_dataPointer--;
                m_instructionPointer++;
            }
            else if(c == '+')
            {
                m_data[m_dataPointer]++;
                m_instructionPointer++;
            }
            else if(c == '-')
            {
                m_data[m_dataPointer]--;
                m_instructionPointer++;
            }
            else if(c == '.')
            {
                m_output.Add((char)m_data[m_dataPointer]);
                m_instructionPointer++;
            }
            else if(c == ',')
            {
                byte input;
                if(m_input != null)
                {
                    input = (byte)m_input[m_inputPointer];
                    m_inputPointer++;
                }
                else
                {
                    input = (byte)Console.Read();
                }
                m_data[m_dataPointer] = input;
                m_instructionPointer++;
            }
            else if(c == '[')
            {
                if(m_data[m_dataPointer] > 0)
                {
                    m_loops.Push(m_instructionPointer);
                }
                else
                {
                    while(m_program[m_instructionPointer] != ']')
                    {
                        m_instructionPointer++;
                    }
                }
                m_instructionPointer++;
            }
            else if(c == ']')
            {
                if(m_data[m_dataPointer] > 0)
                {
                    m_instructionPointer = m_loops.Pop();
                }
                else
                {
                    m_instructionPointer++;
                }
            }
            else
            {
                m_instructionPointer++;
            }
        }
    }
}

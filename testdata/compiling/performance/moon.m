		% Top of stack: r1
		% Global vars: r2
		% Accumulator 1: r3
		% Accumulator 2: r4
		% Current Frame size: r10
		% Parameter passing: r11
		% helper: r13
		% Return value: r14
		% Return address: r15
		%  
		% Compiler.Tac.Instruction
	entry 
	addi r1,r0,st	% Initialize top stack address
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,0
	lw r3,cs(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L0
		% Compiler.Tac.Instruction
	hlt 
L0	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,0
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,4
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L1
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,8
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L2
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,12
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L3
L25	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L1	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,4
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,20
	lw r3,cs(r3)
	addi r4,r0,24
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,16
	lw r3,cs(r3)
	addi r4,r1,16
	lw r4,0(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,28
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L26	% Compiler.Tac.Instruction+Goto
L26	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L2	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,8
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,32
	lw r3,cs(r3)
	addi r4,r0,36
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,24
	lw r3,0(r3)
	addi r4,r0,40
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,44
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L27	% Compiler.Tac.Instruction+Goto
L27	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L3	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,12
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,52
	lw r3,cs(r3)
	addi r4,r0,48
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,56
	lw r3,cs(r3)
	addi r4,r0,60
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L28	% Compiler.Tac.Instruction+Goto
L28	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L4	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,16
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	ceq r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,20
	lw r3,0(r3)
	bz r3,L30
		% Compiler.Tac.Instruction+Write
	addi r3,r0,64
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,68
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,72
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,76
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,80
	lw r3,cs(r3)
	putc r3
	j L31	% Compiler.Tac.Instruction+Goto
L30	
		% Compiler.Tac.Instruction+Write
	addi r3,r0,84
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,88
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,92
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,96
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,100
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,104
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,108
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,112
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,116
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,120
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,124
	lw r3,cs(r3)
	putc r3
L31	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,128
	lw r3,cs(r3)
	add r14,r0,r3
	j L29	% Compiler.Tac.Instruction+Goto
L29	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L5	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,20
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,132
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L33
		% Compiler.Tac.Instruction+Write
	addi r3,r0,136
	lw r3,cs(r3)
	putc r3
	j L34	% Compiler.Tac.Instruction+Goto
L33	
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,140
	lw r4,cs(r4)
	clt r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L35
		% Compiler.Tac.Instruction+Write
	addi r3,r0,144
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,148
	lw r3,cs(r3)
	addi r4,r1,12
	lw r4,0(r4)
	sub r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
	j L36	% Compiler.Tac.Instruction+Goto
L35	
L36	
L34	
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,152
	lw r3,cs(r3)
	add r14,r0,r3
	j L32	% Compiler.Tac.Instruction+Goto
L32	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L6	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,24
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,156
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L38
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,160
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,164
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,168
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Write
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L39	% Compiler.Tac.Instruction+Goto
L38	
L39	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,172
	lw r3,cs(r3)
	add r14,r0,r3
	j L37	% Compiler.Tac.Instruction+Goto
L37	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L7	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,28
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,180
	lw r3,cs(r3)
	addi r4,r0,184
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,176
	lw r3,cs(r3)
	addi r4,r1,16
	lw r4,0(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,188
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L40	% Compiler.Tac.Instruction+Goto
L40	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L8	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,32
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,192
	lw r3,cs(r3)
	addi r4,r0,196
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,24
	lw r3,0(r3)
	addi r4,r0,200
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,204
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L41	% Compiler.Tac.Instruction+Goto
L41	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L9	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,36
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,212
	lw r3,cs(r3)
	addi r4,r0,208
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,216
	lw r3,cs(r3)
	addi r4,r0,220
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L42	% Compiler.Tac.Instruction+Goto
L42	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L10	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,40
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	ceq r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,20
	lw r3,0(r3)
	bz r3,L44
		% Compiler.Tac.Instruction+Write
	addi r3,r0,224
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,228
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,232
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,236
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,240
	lw r3,cs(r3)
	putc r3
	j L45	% Compiler.Tac.Instruction+Goto
L44	
		% Compiler.Tac.Instruction+Write
	addi r3,r0,244
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,248
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,252
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,256
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,260
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,264
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,268
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,272
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,276
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,280
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,284
	lw r3,cs(r3)
	putc r3
L45	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,288
	lw r3,cs(r3)
	add r14,r0,r3
	j L43	% Compiler.Tac.Instruction+Goto
L43	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L11	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,44
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,292
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L47
		% Compiler.Tac.Instruction+Write
	addi r3,r0,296
	lw r3,cs(r3)
	putc r3
	j L48	% Compiler.Tac.Instruction+Goto
L47	
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,300
	lw r4,cs(r4)
	clt r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L49
		% Compiler.Tac.Instruction+Write
	addi r3,r0,304
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,308
	lw r3,cs(r3)
	addi r4,r1,12
	lw r4,0(r4)
	sub r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
	j L50	% Compiler.Tac.Instruction+Goto
L49	
L50	
L48	
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,312
	lw r3,cs(r3)
	add r14,r0,r3
	j L46	% Compiler.Tac.Instruction+Goto
L46	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L12	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,48
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,316
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L52
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,320
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,324
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,328
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Write
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L53	% Compiler.Tac.Instruction+Goto
L52	
L53	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,332
	lw r3,cs(r3)
	add r14,r0,r3
	j L51	% Compiler.Tac.Instruction+Goto
L51	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L13	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,52
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,340
	lw r3,cs(r3)
	addi r4,r0,344
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,336
	lw r3,cs(r3)
	addi r4,r1,16
	lw r4,0(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,348
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L54	% Compiler.Tac.Instruction+Goto
L54	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L14	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,56
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,352
	lw r3,cs(r3)
	addi r4,r0,356
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,24
	lw r3,0(r3)
	addi r4,r0,360
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,364
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L55	% Compiler.Tac.Instruction+Goto
L55	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L15	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,60
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,372
	lw r3,cs(r3)
	addi r4,r0,368
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,376
	lw r3,cs(r3)
	addi r4,r0,380
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L56	% Compiler.Tac.Instruction+Goto
L56	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L16	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,64
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	ceq r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,20
	lw r3,0(r3)
	bz r3,L58
		% Compiler.Tac.Instruction+Write
	addi r3,r0,384
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,388
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,392
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,396
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,400
	lw r3,cs(r3)
	putc r3
	j L59	% Compiler.Tac.Instruction+Goto
L58	
		% Compiler.Tac.Instruction+Write
	addi r3,r0,404
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,408
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,412
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,416
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,420
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,424
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,428
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,432
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,436
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,440
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,444
	lw r3,cs(r3)
	putc r3
L59	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,448
	lw r3,cs(r3)
	add r14,r0,r3
	j L57	% Compiler.Tac.Instruction+Goto
L57	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L17	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,68
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,452
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L61
		% Compiler.Tac.Instruction+Write
	addi r3,r0,456
	lw r3,cs(r3)
	putc r3
	j L62	% Compiler.Tac.Instruction+Goto
L61	
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,460
	lw r4,cs(r4)
	clt r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L63
		% Compiler.Tac.Instruction+Write
	addi r3,r0,464
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,468
	lw r3,cs(r3)
	addi r4,r1,12
	lw r4,0(r4)
	sub r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
	j L64	% Compiler.Tac.Instruction+Goto
L63	
L64	
L62	
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,472
	lw r3,cs(r3)
	add r14,r0,r3
	j L60	% Compiler.Tac.Instruction+Goto
L60	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L18	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,72
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,476
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L66
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,480
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,484
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,488
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Write
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L67	% Compiler.Tac.Instruction+Goto
L66	
L67	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,492
	lw r3,cs(r3)
	add r14,r0,r3
	j L65	% Compiler.Tac.Instruction+Goto
L65	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L19	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,76
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,500
	lw r3,cs(r3)
	addi r4,r0,504
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,496
	lw r3,cs(r3)
	addi r4,r1,16
	lw r4,0(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,508
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L68	% Compiler.Tac.Instruction+Goto
L68	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L20	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,80
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,512
	lw r3,cs(r3)
	addi r4,r0,516
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,24
	lw r3,0(r3)
	addi r4,r0,520
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r0,524
	lw r4,cs(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L69	% Compiler.Tac.Instruction+Goto
L69	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L21	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,84
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,532
	lw r3,cs(r3)
	addi r4,r0,528
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,536
	lw r3,cs(r3)
	addi r4,r0,540
	lw r4,cs(r4)
	sub r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L4
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r14	% Put the returned value into r3
	add r14,r0,r3
	j L70	% Compiler.Tac.Instruction+Goto
L70	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L22	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,88
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r1,16
	lw r4,0(r4)
	ceq r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,20
	lw r3,0(r3)
	bz r3,L72
		% Compiler.Tac.Instruction+Write
	addi r3,r0,544
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,548
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,552
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,556
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,560
	lw r3,cs(r3)
	putc r3
	j L73	% Compiler.Tac.Instruction+Goto
L72	
		% Compiler.Tac.Instruction+Write
	addi r3,r0,564
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,568
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,572
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,576
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,580
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,584
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,588
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,592
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,596
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Write
	addi r3,r0,600
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,16
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L5
		% Compiler.Tac.Instruction+Write
	addi r3,r0,604
	lw r3,cs(r3)
	putc r3
L73	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,608
	lw r3,cs(r3)
	add r14,r0,r3
	j L71	% Compiler.Tac.Instruction+Goto
L71	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L23	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,92
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,612
	lw r4,cs(r4)
	ceq r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L75
		% Compiler.Tac.Instruction+Write
	addi r3,r0,616
	lw r3,cs(r3)
	putc r3
	j L76	% Compiler.Tac.Instruction+Goto
L75	
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,620
	lw r4,cs(r4)
	clt r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,16
	lw r3,0(r3)
	bz r3,L77
		% Compiler.Tac.Instruction+Write
	addi r3,r0,624
	lw r3,cs(r3)
	putc r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r0,628
	lw r3,cs(r3)
	addi r4,r1,12
	lw r4,0(r4)
	sub r3,r3,r4
	addi r13,r1,12
	sw 0(r13),r3
	j L78	% Compiler.Tac.Instruction+Goto
L77	
L78	
L76	
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,12
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,632
	lw r3,cs(r3)
	add r14,r0,r3
	j L74	% Compiler.Tac.Instruction+Goto
L74	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
L24	
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	add r3,r3,r4
	add r1,r0,r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r15	% Put the return address into r3
	addi r13,r1,0
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	add r3,r0,r1	% Put the address of the top of the stack in r3
	add r4,r0,r10	% Put the current frame's size value into r4
	sub r3,r3,r4
	addi r13,r1,4
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	add r3,r0,r10	% Put the current frame's size value into r3
	addi r13,r1,8
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,96
	lw r3,fz(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,636
	lw r4,cs(r4)
	cne r3,r3,r4
	addi r13,r1,24
	sw 0(r13),r3
		% Compiler.Tac.Instruction+IfFalse
	addi r3,r1,24
	lw r3,0(r3)
	bz r3,L80
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,640
	lw r4,cs(r4)
	mod r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,12
	lw r3,0(r3)
	addi r4,r0,644
	lw r4,cs(r4)
	div r3,r3,r4
	addi r13,r1,20
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Push
	add r3,r1,r10	% Find the start of the next frame by adding the top of the stack to the current frame's size
	addi r3,r3,12	% Add 12 for the head of the of the next frame
	add r3,r3,r11	% Add the parameter passing register's offset to that
	addi r4,r1,20
	lw r4,0(r4)
	sw 0(r3),r4	% Store the argument on the stack
	addi r11,r11,4	% Add 4 to the parameter passing register
		% Compiler.Tac.Instruction+Call
	add r11,r0,r0	% Clear the prameter passing register
	jl r15,L6
		% Compiler.Tac.Instruction+Inst3
	addi r3,r1,16
	lw r3,0(r3)
	addi r4,r0,648
	lw r4,cs(r4)
	add r3,r3,r4
	addi r13,r1,16
	sw 0(r13),r3
		% Compiler.Tac.Instruction+Write
	addi r3,r1,16
	lw r3,0(r3)
	putc r3
	j L81	% Compiler.Tac.Instruction+Goto
L80	
L81	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,652
	lw r3,cs(r3)
	add r14,r0,r3
	j L79	% Compiler.Tac.Instruction+Goto
L79	
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,8
	lw r3,0(r3)
	add r10,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,0
	lw r3,0(r3)
	add r15,r0,r3
		% Compiler.Tac.Instruction+Assign
	addi r3,r1,4
	lw r3,0(r3)
	add r1,r0,r3
	jr r15	% Compiler.Tac.Instruction
fz	dw 12
	dw 32
	dw 28
	dw 28
	dw 28
	dw 24
	dw 32
	dw 32
	dw 28
	dw 28
	dw 28
	dw 24
	dw 32
	dw 32
	dw 28
	dw 28
	dw 28
	dw 24
	dw 32
	dw 32
	dw 28
	dw 28
	dw 28
	dw 24
	dw 32
cs	dw 0
	dw 0
	dw 0
	dw 0
	dw 3
	dw 10
	dw 10
	dw 4
	dw 3
	dw 10
	dw 10
	dw 1
	dw 10
	dw 0
	dw 0
	dw 10
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 45
	dw 62
	dw 32
	dw 32
	dw 58
	dw 32
	dw 10
	dw 0
	dw 0
	dw 48
	dw 0
	dw 45
	dw 0
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
	dw 3
	dw 10
	dw 10
	dw 4
	dw 3
	dw 10
	dw 10
	dw 1
	dw 10
	dw 0
	dw 0
	dw 10
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 45
	dw 62
	dw 32
	dw 32
	dw 58
	dw 32
	dw 10
	dw 0
	dw 0
	dw 48
	dw 0
	dw 45
	dw 0
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
	dw 3
	dw 10
	dw 10
	dw 4
	dw 3
	dw 10
	dw 10
	dw 1
	dw 10
	dw 0
	dw 0
	dw 10
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 45
	dw 62
	dw 32
	dw 32
	dw 58
	dw 32
	dw 10
	dw 0
	dw 0
	dw 48
	dw 0
	dw 45
	dw 0
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
	dw 3
	dw 10
	dw 10
	dw 4
	dw 3
	dw 10
	dw 10
	dw 1
	dw 10
	dw 0
	dw 0
	dw 10
	dw 80
	dw 65
	dw 83
	dw 83
	dw 10
	dw 70
	dw 65
	dw 73
	dw 76
	dw 45
	dw 62
	dw 32
	dw 32
	dw 58
	dw 32
	dw 10
	dw 0
	dw 0
	dw 48
	dw 0
	dw 45
	dw 0
	dw 0
	dw 0
	dw 10
	dw 10
	dw 48
	dw 0
gb	res 0
st	res 10000	% The stack

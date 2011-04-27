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
L7	
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
	j L8	% Compiler.Tac.Instruction+Goto
L8	
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
	j L9	% Compiler.Tac.Instruction+Goto
L9	
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
	j L10	% Compiler.Tac.Instruction+Goto
L10	
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
	bz r3,L12
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
	j L13	% Compiler.Tac.Instruction+Goto
L12	
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
L13	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,128
	lw r3,cs(r3)
	add r14,r0,r3
	j L11	% Compiler.Tac.Instruction+Goto
L11	
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
	bz r3,L15
		% Compiler.Tac.Instruction+Write
	addi r3,r0,136
	lw r3,cs(r3)
	putc r3
	j L16	% Compiler.Tac.Instruction+Goto
L15	
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
	bz r3,L17
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
	j L18	% Compiler.Tac.Instruction+Goto
L17	
L18	
L16	
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
	j L14	% Compiler.Tac.Instruction+Goto
L14	
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
	bz r3,L20
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
	j L21	% Compiler.Tac.Instruction+Goto
L20	
L21	
		% Compiler.Tac.Instruction+Assign
	addi r3,r0,172
	lw r3,cs(r3)
	add r14,r0,r3
	j L19	% Compiler.Tac.Instruction+Goto
L19	
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
gb	res 0
st	res 10000	% The stack
